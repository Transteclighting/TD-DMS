using CrystalDecisions.Web;
using DealerManagementSystem.Models;
using DealerManagementSystem.Models.Helper;
using DealerManagementSystem.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DealerManagementSystem.Controllers
{
    public class GoodReceiveController : Controller
    {
        private readonly ProjectDb _db;
        public int tranId = 0;
        public GoodReceiveController()
        {
            _db = new ProjectDb();
        }
        public ActionResult Index()
        {
            if (Session["CustomerID"] != null)
            {

                int nCustomerId = (int)Session["CustomerID"];

                string sql = @"Select TranNo as InvoiceNo,IsDistSysDBTran From 
                (
                ---t_DMSProductStockTran---
                Select InvoiceDate,a.CustomerID,InvoiceNo as TranNo,0 as IsDistSysDBTran 
                From t_SalesInvoice a
                join t_DMSOutlet c on a.CustomerID=c.CustomerID
                where InvoiceNo not in 
                (Select TranNo From t_DMSProductStockTran 
                where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=0)
                Union All
                Select InvoiceDate,bb.CustomerID,InvoiceNo as TranNo,1 as IsDistSysDBTran  
                From [10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
                join [10.168.14.36].DistsysDB.dbo.t_Customer b on a.CustomerID=b.CustomerID
                join dbo.t_Customer bb on b.CustomerCode=bb.CustomerCode
                join t_DMSOutlet c on bb.CustomerID=c.CustomerID
                where InvoiceNo not in 
                (Select TranNo From t_DMSProductStockTran 
                where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=1)
                ) x where InvoiceDate>='01-SEP-2022' and CustomerID=" + nCustomerId + "";

                // " + nCustomerId + "
                var allInvoice = _db.Database.SqlQuery<Invoice>(sql).ToList();
                ViewBag.All_Invoice = new SelectList(allInvoice, "InvoiceNo", "InvoiceNo");
                Session["InvoiceList"] = allInvoice;


                if (TempData["StockInMsg"] != null && TempData["StockInStatus"] != null)
                {
                    ViewBag.StockInStatus = TempData["StockInStatus"].ToString();
                    ViewBag.StockInMsg = TempData["StockInMsg"].ToString();
                    ViewBag.CreateMsg = TempData["StockInMsg"].ToString();
                }
                return View();
            }
            return RedirectToAction("Login", "User");
        }
        public JsonResult GetProducts(string InvoiceNo)
        {

            List<Invoice> inv = new List<Invoice>();
            inv = (List<Invoice>)Session["InvoiceList"];
            int IsDistSysDBTran = inv.FirstOrDefault(x => x.InvoiceNo == InvoiceNo).IsDistSysDBTran;
            var nCustomerId = (int)Session["CustomerID"];
            string sql = "";
            if (IsDistSysDBTran == 1)
            {
                sql = @"
                    Select g.ProductCode,g.ProductName,ProductSerialNo
                    --g.ProductModel,,1 as Status,ProductSerialNo as OriginalSerial,g.ProductID,
                    From [10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
                    join [10.168.14.36].DistsysDB.dbo.t_SalesInvoiceProductSerial b on a.InvoiceID=b.InvoiceID
                    join [10.168.14.36].DistsysDB.dbo.t_Customer c on a.CustomerID=c.CustomerID
                    join dbo.t_Customer d on c.CustomerCode=d.CustomerCode
                    join t_DMSOutlet e on e.CustomerID=d.CustomerID
                    join [10.168.14.36].DistsysDB.dbo.t_Product f on b.ProductID=f.ProductID
                    join dbo.t_Product g on f.ProductCode=g.ProductCode
                    where a.InvoiceNo not in 
                    (
                    Select TranNo From t_DMSProductStockTran 
                    where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=1
                    ) and InvoiceDate>='01-SEP-2022' and d.CustomerID=" + nCustomerId + @"
                    and InvoiceNo='" + InvoiceNo + @"'";
            }
            else
            {
                sql = @"Select ProductCode,ProductName,ProductSerialNo
                --b.ProductID,,1 as Status,ProductSerialNo as OriginalSerial,ProductModel,
                From t_SalesInvoice a
                join t_SalesInvoiceProductSerial b on a.InvoiceID=b.InvoiceID
                join t_DMSOutlet c on a.CustomerID=c.CustomerID
                join t_Product d on b.ProductID=d.ProductID
                where InvoiceNo not in 
                (
                Select TranNo From t_DMSProductStockTran 
                where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=0
                ) and InvoiceDate>='01-SEP-2022' and a.CustomerID=" + nCustomerId + @"
                and InvoiceNo='" + InvoiceNo + "'";

            }
            var allInvoice = _db.Database.SqlQuery<GoodReceiveViewModel>(sql).ToList();
            return Json(allInvoice, JsonRequestBehavior.AllowGet);
        }
        public void SaveProduct(string InvoiceNo)
        {
            try
            {

                Insert_DMSproductStockTran(InvoiceNo);
                var transaction = _db.Database.BeginTransaction();
                Insert_DMSproductStockTranItem(InvoiceNo);
                Insert_DMSproductSerial(InvoiceNo);
                Update_Stock(InvoiceNo);
                transaction.Commit();
                _db.SaveChanges();

                TempData["StockInStatus"] = "1";
                TempData["StockInMsg"] = "Stock Successfully Inserted !!.";
                //  ViewBag.Success("Stock In Successfull");
            }
            catch (Exception ex)
            {
                TempData["StockInStatus"] = "2";
                TempData["StockInMsg"] = "Stock In Failed.Try Again.";

                AppLogger.PrintException(ex, "");
            }

            RedirectToAction("Index");

        }
        public void Insert_DMSproductStockTran(string InvoiceNo)
        {
            var nCustomerId = (int)Session["CustomerID"];
            string sql = @"           
            Select TranNo,TranDate,TranType,TranSide,FromCustomerID,
            ToCustomerID,Remarks,CreateDate,RefInvoiceId,InvoiceAmount,
            DiscountAmount,IsDistSysDBTran
            From 
            (
            ---t_DMSProductStockTran---
            Select InvoiceNo as TranNo,
            GETDATE() as TranDate,
            Convert(int,1) as TranType,convert(int,1) as TranSide,
            convert(int,-1) FromCustomerID,
            a.CustomerID as ToCustomerID,Remarks,
            GETDATE() CreateDate,
            convert(int,isnull(RefInvoiceId,0)) as RefInvoiceId,
            InvoiceAmount,
            Discount as DiscountAmount,convert(int,0) as IsDistSysDBTran,
            InvoiceDate
            From t_SalesInvoice a
            join t_DMSOutlet c on a.CustomerID=c.CustomerID
            where InvoiceNo not in 
            (Select TranNo From t_DMSProductStockTran 
            where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=0)
            Union All
            Select InvoiceNo as TranNo,
            GETDATE() as TranDate,
            Convert(int,1) as TranType,convert(int,1) as TranSide,
            convert(int,-1) FromCustomerID,
            bb.CustomerID as ToCustomerID,Remarks,
            GETDATE() as CreateDate,
            convert(int,isnull(RefInvoiceId,0)) as RefInvoiceId,
            InvoiceAmount,Discount as DiscountAmount,convert(int,1) as IsDistSysDBTran,
            InvoiceDate
            From [10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
            join [10.168.14.36].DistsysDB.dbo.t_Customer b on a.CustomerID=b.CustomerID
            join dbo.t_Customer bb on b.CustomerCode=bb.CustomerCode
            join t_DMSOutlet c on bb.CustomerID=c.CustomerID
            where InvoiceNo not in 
            (Select TranNo From t_DMSProductStockTran 
            where TranSide=1 and TranType=1 and isnull(IsDistSysDBTran,0)=1)
            ) x where InvoiceDate>='01-SEP-2022' and ToCustomerID=" + nCustomerId + " and TranNo='" + InvoiceNo + "'";
            var allInvoice = _db.Database.SqlQuery<DmsProductStockTran_A>(sql).ToList();
            DmsProductStockTran TranList = new DmsProductStockTran();
            foreach (var item in allInvoice)
            {
                tranId = _db.DmsProductStockTran.DefaultIfEmpty().Max(r => r == null ? 1 : r.TranId + 1);
                TranList.TranId = tranId;
                TranList.TranNo = item.TranNo;
                TranList.TranDate = item.TranDate;
                TranList.TranType = item.TranType;
                TranList.TranSide = item.TranSide;
                TranList.FromCustomerId = item.FromCustomerId;
                TranList.ToCustomerId = item.ToCustomerId;
                TranList.Remarks = item.Remarks;
                TranList.CreateDate = item.CreateDate;
                TranList.RefInvoiceId = item.RefInvoiceId;
                TranList.InvoiceAmount = item.InvoiceAmount;
                TranList.DiscountAmount = item.DiscountAmount;
                TranList.IsDistSysDBTran = item.IsDistSysDBTran;
                _db.DmsProductStockTran.Add(TranList);
            }
            _db.SaveChanges();
            //_db.DmsProductStockTran.Add(TranList);
            //_db.SaveChanges();
        }
        public void Insert_DMSproductStockTranItem(string InvoiceNo)
        {
            var nCustomerId = (int)Session["CustomerID"];
            string sql = "";
            List<Invoice> inv = new List<Invoice>();
            inv = (List<Invoice>)Session["InvoiceList"];
            int IsDistSysDBTran = inv.FirstOrDefault(x => x.InvoiceNo == InvoiceNo).IsDistSysDBTran;
            if (IsDistSysDBTran == 0)
            {
                sql = @"
                Select b.ProductID,convert(int,(Quantity+FreeQty)) as Qty,CostPrice,UnitPrice as SalesPrice
                From t_SalesInvoice a
                join t_SalesInvoiceDetail b on a.InvoiceID=b.InvoiceID
                join t_Product d on b.ProductID=d.ProductID
                where InvoiceDate>='01-SEP-2022' and IsBarcodeItem=1 
                and InvoiceNo='" + InvoiceNo + "' and Customerid=" + nCustomerId + "";
            }
            else
            {
                sql = @"Select e.ProductID,convert(int, (Quantity + FreeQty)) as Qty,CostPrice,UnitPrice as SalesPrice
                From[10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
                join[10.168.14.36].DistsysDB.dbo.t_SalesInvoiceDetail b on a.InvoiceID = b.InvoiceID
                join[10.168.14.36].DistsysDB.dbo.t_Product d on b.ProductID = d.ProductID
                join dbo.t_Product e on d.ProductCode = e.ProductCode
                where InvoiceDate>= '01-SEP-2022' and e.IsBarcodeItem = 1
                and InvoiceNo = '" + InvoiceNo + "' and customerID=" + nCustomerId + "";
            }
            var AllItem = _db.Database.SqlQuery<DmsProductStockTranItem_A>(sql).ToList();
            foreach (var item in AllItem)
            {
                int TranID = _db.DmsProductStockTran.Where(u => u.TranNo == InvoiceNo).Select(u => u.TranId).SingleOrDefault();
                var ItemList = new DmsProductStockTranItem
                {
                    TranId = TranID,
                    ProductId = item.ProductId,
                    Qty = item.Qty,
                    CostPrice = item.CostPrice,
                    SalesPrice = item.SalesPrice
                };
                _db.DmsProductStockTranItems.Add(ItemList);
            }
        }
        public void Insert_DMSproductSerial(string InvoiceNo)
        {
            var nCustomerId = (int)Session["CustomerID"];
            string sql = "";
            List<Invoice> inv = new List<Invoice>();
            inv = (List<Invoice>)Session["InvoiceList"];
            int IsDistSysDBTran = inv.FirstOrDefault(x => x.InvoiceNo == InvoiceNo).IsDistSysDBTran;
            if (IsDistSysDBTran == 1)
            {
                sql = @"Select g.ProductID,ProductSerialNo as ProductSerial,1 as Status,ProductSerialNo as OriginalSerial
                From [10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
                join [10.168.14.36].DistsysDB.dbo.t_SalesInvoiceProductSerial b on a.InvoiceID=b.InvoiceID
                join [10.168.14.36].DistsysDB.dbo.t_Customer c on a.CustomerID=c.CustomerID
                join dbo.t_Customer d on c.CustomerCode=d.CustomerCode
                join t_DMSOutlet e on e.CustomerID=d.CustomerID
                join [10.168.14.36].DistsysDB.dbo.t_Product f on b.ProductID=f.ProductID
                join dbo.t_Product g on f.ProductCode=g.ProductCode
                where InvoiceDate>='01-SEP-2022' and d.CustomerID=" + nCustomerId + @"
                and InvoiceNo='" + InvoiceNo + @"'";
            }
            else
            {
                sql = @"Select b.ProductID,ProductSerialNo as ProductSerial,1 as Status,ProductSerialNo as OriginalSerial
                From t_SalesInvoice a
                join t_SalesInvoiceProductSerial b on a.InvoiceID=b.InvoiceID
                join t_DMSOutlet c on a.CustomerID=c.CustomerID
                join t_Product d on b.ProductID=d.ProductID
                where InvoiceDate>='01-SEP-2022' and a.CustomerID=" + nCustomerId + @"
                and InvoiceNo='" + InvoiceNo + "'";
            }
            var AllItem = _db.Database.SqlQuery<DmsProductStockSerial_A>(sql).ToList();


            foreach (var item in AllItem)
            {
                int TranID = _db.DmsProductStockTran.Where(u => u.TranNo == InvoiceNo).Select(u => u.TranId).SingleOrDefault();

                var ItemList = new DmsProductStockSerial
                {
                    TranId = TranID,
                    ProductId = item.ProductId,
                    ProductSerial = item.ProductSerial,
                    Status = item.Status,
                    OriginalSerial = item.OriginalSerial,
                };
                _db.DmsProductStockSerials.Add(ItemList);
            }

            //  _db.SaveChanges();

        }
        public void Update_Stock(string InvoiceNo)
        {
            var nCustomerId = (int)Session["CustomerID"];
            List<Invoice> inv = new List<Invoice>();
            inv = (List<Invoice>)Session["InvoiceList"];
            int IsDistSysDBTran = inv.FirstOrDefault(x => x.InvoiceNo == InvoiceNo).IsDistSysDBTran;
            string sql = "";
            if (IsDistSysDBTran == 1)
            {
                sql = @"Select b.ProductID,customerID as DistributorID,convert(int,(Quantity+FreeQty)) as CurrentStock
                --,CostPrice,UnitPrice as SalesPrice
                From[10.168.14.36].DistsysDB.dbo.t_SalesInvoice a
                join[10.168.14.36].DistsysDB.dbo.t_SalesInvoiceDetail b on a.InvoiceID = b.InvoiceID
                join[10.168.14.36].DistsysDB.dbo.t_Product d on b.ProductID = d.ProductID
                join dbo.t_Product e on d.ProductCode = e.ProductCode
                where InvoiceDate>= '01-SEP-2022' and e.IsBarcodeItem = 1
                and InvoiceNo = '" + InvoiceNo + "' and customerID=" + nCustomerId + "";
            }
            else
            {
                sql = @"
                Select b.ProductID,customerID as DistributorID,convert(int,(Quantity+FreeQty)) as CurrentStock
                --,CostPrice,UnitPrice as SalesPrice
                From t_SalesInvoice a
                join t_SalesInvoiceDetail b on a.InvoiceID=b.InvoiceID
                join t_Product d on b.ProductID=d.ProductID
                where InvoiceDate>='01-SEP-2022' and IsBarcodeItem=1 
                and InvoiceNo='" + InvoiceNo + "' and Customerid=" + nCustomerId + "";
            }
            var AllItem = _db.Database.SqlQuery<DmsProductStock_A>(sql).ToList();
            foreach (var item in AllItem)
            {
                if (_db.DmsProductStocks.Any(o => o.ProductId == item.ProductId && o.DistributorId == item.DistributorId))
                {
                    DmsProductStock c = _db.DmsProductStocks.First(i => i.DistributorId == item.DistributorId && i.ProductId == item.ProductId);
                    c.CurrentStock = c.CurrentStock + item.CurrentStock;
                }
                else
                {
                    var ItemList = new DmsProductStock
                    {
                        ProductId = item.ProductId,
                        DistributorId = item.DistributorId,
                        CurrentStock = item.CurrentStock,
                    };
                    _db.DmsProductStocks.Add(ItemList);
                }
            }
        }
    }
}