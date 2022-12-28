using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using DealerManagementSystem.Models;
using System.Data.Entity.SqlServer;
using DealerManagementSystem.Enums;
using System.Transactions;
using DealerManagementSystem.Models.Helper;
using DealerManagementSystem.Models.ViewModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace DealerManagementSystem.Controllers
{
    public class DmsSalesInvoiceController : Controller
    {

        private readonly ProjectDb _db = new ProjectDb();
        public ActionResult Index()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                DateTime toDay = DateTime.Today;
                var nCustomerId = (int)Session["CustomerID"];
                ViewBag.UserType = Session["UserType"].ToString();
                var salesInvoices = _db.DmsSalesInvoices.Where(a => a.CustomerId == nCustomerId && a.InvoiceDate >= toDay)
                                     .OrderByDescending(a => a.InvoiceId).Include(b => b.Consumer).ToList();
                if (TempData["InvoMakeMsg"] != null && TempData["InvoMakeStatus"] != null)
                {
                    ViewBag.InvoMakeStatus = TempData["InvoMakeStatus"].ToString();
                    ViewBag.InvoiceInsertionStatus = TempData["InvoMakeMsg"].ToString();
                }
                return View(salesInvoices);
            }
            return RedirectToAction("Login", "User");
        }
        public JsonResult GetInvoice(string sStartDate, string sEndDate)
        {
            var nCustomerId = (int)Session["CustomerID"];
            var dtStartDate = DateTime.Parse(sStartDate).Date;
            var dtEndDate = DateTime.Parse(sEndDate).Date;

            var salesInvoices = (from a in _db.DmsSalesInvoices
                                 join b in _db.Consumers on a.ConsumerId equals b.ConsumerId
                                 where a.CustomerId == nCustomerId && a.InvoiceDate >= dtStartDate && a.InvoiceDate <= dtEndDate
                                 orderby a.InvoiceId descending
                                 select new
                                 {
                                     a.InvoiceNo,
                                     InvoiceDay = SqlFunctions.DateName("dd", a.InvoiceDate),
                                     InvoiceMonth = SqlFunctions.DateName("mm", a.InvoiceDate),
                                     InvoiceYear = SqlFunctions.DateName("yyyy", a.InvoiceDate),
                                     a.InvoiceAmount,
                                     a.InvoiceId,
                                     b.Address,
                                     b.ConsumerName,
                                     b.ContactNo
                                 }).ToList();
            return Json(salesInvoices, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MakeInvoice()
        {

            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                var aInvoices = new InvoiceViewModel
                {
                    DmsDiscountTypes = _db.DmsDiscountTypes.Where(a => a.IsActive).ToList()
                };
                ViewBag.products = (List<ProductViewModel>)Session["selectedProducts"];
                if (Session["UserType"].ToString() == "DB")
                {
                    var nCustomerId = (int)Session["CustomerID"];
                    string sGetRoute = "Select RouteID,RouteName From t_DMSRoute where DistributorID=" + nCustomerId + "";
                    var allRoute = _db.Routes.SqlQuery(sGetRoute).ToList();
                    ViewBag.Routelist = new SelectList(allRoute, "RouteID", "RouteName");

                    string sGetOutlet = "Select RetailID,RetailName,RetailAddress,Mobile01,Email  From t_DMSCLusterOutlet where CustomerID=" + nCustomerId + "";
                    var allOutlet = _db.Outlets.SqlQuery(sGetOutlet).ToList();
                    ViewBag.OutletList = new SelectList(allOutlet, "RetailID", "RetailName");
                    ViewBag.Cosumer = allOutlet.ToList();
                }
                else
                {
                    ViewBag.RouteList = new SelectList("", "");
                    ViewBag.OutletList = new SelectList("", "");
                }



                string sGetBank = "Select BankID,Name From t_Bank";
                var allBank = _db.Banks.SqlQuery(sGetBank).ToList();
                ViewBag.BankList = new SelectList(allBank, "BankID", "Name");

                string sGetPOS = "Select AssetID,AssetName From t_ShowroomAsset";
                var allPOSMachine = _db.POSMachine.SqlQuery(sGetPOS).ToList();
                ViewBag.POSMachine = new SelectList(allPOSMachine, "AssetID", "Assetname");

                string sGetInstellmentNo = "Select distinct Tenure From t_EMITenure";
                var allins = _db.InstallmentNo.SqlQuery(sGetInstellmentNo).ToList();
                ViewBag.InstallmentList = new SelectList(allins, "Tenure", "Tenure");

                string sGetPaymentMode = "Select PaymentModeID,PaymentModeName From t_PaymentMode where IsEligibleForDMS=1";
                var allPayMode = _db.PaymentMode.SqlQuery(sGetPaymentMode).ToList();
                ViewBag.PayModeList = new SelectList(allPayMode, "PaymentModeID", "PaymentModeName");

                var enumData = from CreditCardType e in Enum.GetValues(typeof(CreditCardType))
                               select new
                               {
                                   ID = (int)e,
                                   Name = e.ToString()
                               };

                ViewBag.EnumList = new SelectList(enumData, "ID", "Name");


                var CardCategory = from CardCategory e in Enum.GetValues(typeof(CardCategory))
                                   select new
                                   {
                                       ID = (int)e,
                                       Name = e.ToString()
                                   };

                ViewBag.CategoryList = new SelectList(CardCategory, "ID", "Name");
                return View(aInvoices);
            }
            return RedirectToAction("Login", "User");
        }
        public ActionResult MakeInvoice_FromOrder(int TranID)
        {

            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                var aInvoices = new InvoiceViewModel
                {
                    DmsDiscountTypes = _db.DmsDiscountTypes
                                       .Where(a => a.IsActive).ToList()
                };
                ViewBag.products = (List<ProductViewModel>)Session["selectedProducts"];



                //if (Session["UserType"].ToString() == "DB")
                //{

                var nCustomerId = (int)Session["CustomerID"];
                string sGetRoute = "Select RouteID,RouteName From t_DMSRoute a, t_DMSOrder b where a.routeid=b.beatid and TranID=" + TranID + "";
                var allRoute = _db.Routes.SqlQuery(sGetRoute).ToList();
                ViewBag.Routelist = new SelectList(allRoute, "RouteID", "RouteName");


                string sGetOutlet = "Select RetailID, RetailName ,RetailAddress,Mobile01,Email  From t_DMSOrder a, t_DMSCLusterOutlet b  where a.OutletID=b.Retailid and TranID=" + TranID + "";
                var allOutlet = _db.Outlets.SqlQuery(sGetOutlet).ToList();
                // ViewBag.OutletList = new SelectList(allOutlet, "RetailID", "RetailName");
                //ViewBag.RetailDetail = JsonConvert.SerializeObject(allOutlet);
                ViewBag.RetailDetail = allOutlet;





                string sGetBank = "Select BankID,Name From t_Bank";
                var allBank = _db.Banks.SqlQuery(sGetBank).ToList();
                ViewBag.BankList = new SelectList(allBank, "BankID", "Name");

                string sGetPOS = "Select AssetID,AssetName From t_ShowroomAsset";
                var allPOSMachine = _db.POSMachine.SqlQuery(sGetPOS).ToList();
                ViewBag.POSMachine = new SelectList(allPOSMachine, "AssetID", "Assetname");

                string sGetInstellmentNo = "Select distinct Tenure From t_EMITenure";
                var allins = _db.InstallmentNo.SqlQuery(sGetInstellmentNo).ToList();
                ViewBag.InstallmentList = new SelectList(allins, "Tenure", "Tenure");

                string sGetPaymentMode = "Select PaymentModeID,PaymentModeName From t_PaymentMode where IsEligibleForDMS=1";
                var allPayMode = _db.PaymentMode.SqlQuery(sGetPaymentMode).ToList();
                ViewBag.PayModeList = new SelectList(allPayMode, "PaymentModeID", "PaymentModeName");

                var enumData = from CreditCardType e in Enum.GetValues(typeof(CreditCardType))
                               select new
                               {
                                   ID = (int)e,
                                   Name = e.ToString()
                               };

                ViewBag.EnumList = new SelectList(enumData, "ID", "Name");


                var CardCategory = from CardCategory e in Enum.GetValues(typeof(CardCategory))
                                   select new
                                   {
                                       ID = (int)e,
                                       Name = e.ToString()
                                   };

                ViewBag.CategoryList = new SelectList(CardCategory, "ID", "Name");
                return View("MakeInvoice", aInvoices);
            }
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeInvoice(InvoiceViewModel aInvoiceViewModel)
        {
            try
            {
                using (var ts = new TransactionScope())
                {
                    aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount = 0;
                    int nCustomerId = (int)Session["CustomerId"];
                    var barcodes = aInvoiceViewModel.DmsSalesInvoiceDetail.Select(a => a.BarcodeSerial).ToList();
                    var productStocks = (from ab in _db.DmsProductStockSerials
                                         join b in _db.ProductDetails on ab.ProductId equals b.ProductId
                                         join c in _db.DmsProductStockTran on ab.TranId equals c.TranId
                                         where barcodes.Contains(ab.ProductSerial)
                                         && ab.Status == 1
                                         && c.ToCustomerId == nCustomerId
                                         select new { ab.ProductId, ab.ProductSerial, b.Rsp, b.CostPrice }).ToList();
                    if (productStocks.Count != aInvoiceViewModel.DmsSalesInvoiceDetail.Count)
                    {
                        TempData["InvoMakeStatus"] = "2";
                        TempData["InvoMakeMsg"] = "Please select valid product code";
                    }
                    else
                    {
                        var invoiceBarcode = aInvoiceViewModel.DmsSalesInvoiceDetail.FindAll(a => a.IsFreeProduct != true)
                                .Select(b => b.BarcodeSerial)
                                .ToList();
                        var invoiceDiscounts = aInvoiceViewModel.DmsInvoiceDiscount.Where(a => a.Amount > 0).ToList();
                        aInvoiceViewModel.DmsSalesInvoice.DiscountAmount = invoiceDiscounts.Sum(a => a.Amount);

                        foreach (string aBarcode in invoiceBarcode)
                        {
                            aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount += productStocks.First(a => a.ProductSerial == aBarcode).Rsp;
                        }

                        aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount -=
                            aInvoiceViewModel.DmsSalesInvoice.DiscountAmount;
                        int consumerId = InsertConsumerAndGetConsumerId(aInvoiceViewModel.Consumer);

                        //int consumerId = 0;
                        //if (Session["UserType"].ToString()=="DB")
                        //{
                        //    consumerId = InsertConsumerAndGetConsumerId_DB();
                        //}
                        //else
                        //{
                        //    consumerId = InsertConsumerAndGetConsumerId(aInvoiceViewModel.Consumer);
                        //}


                        var invoice = InsertInvoiceAndGetInvoiceId(consumerId, aInvoiceViewModel.DmsSalesInvoice, aInvoiceViewModel);
                        int tranId = InsertDmsProductStockTranAndGetTrandId(aInvoiceViewModel.DmsSalesInvoice.Remarks,
                            invoice.InvoiceId);

                        InsertInvoiceDetails(invoice.InvoiceId, tranId, aInvoiceViewModel.DmsSalesInvoiceDetail);
                        GetWarranyParamAndInsertWarranty(invoice.InvoiceId);

                        invoiceDiscounts.ForEach(a => a.InvoiceId = invoice.InvoiceId);
                        InsertInvoiceDiscount(invoiceDiscounts);

                        var invoiceProductBarcodes = aInvoiceViewModel.DmsSalesInvoiceDetail
                                                    .Where(a => !a.IsFreeProduct)
                                                    .Select(b => b.BarcodeSerial)
                                                    .ToList();
                        var invoiceProductIds = productStocks.Where(a => invoiceProductBarcodes.Contains(a.ProductSerial))
                                                      .Select(c => c.ProductId)
                                                      .ToList();

                        InsertPromoDetails(invoiceProductIds, invoice.InvoiceId);

                        //PaymentDetails pp = new PaymentDetails();
                        //pp.InvoiceID = invoice.InvoiceId;
                        double Netpayble = invoice.InvoiceAmount;
                        double Paid = Convert.ToDouble(Session["PaidAmount"]);

                        if (Netpayble == Paid)
                        {
                            InsertPaymentDetails(invoice.InvoiceId);
                            _db.SaveChanges();
                            ts.Complete();

                            TempData["InvoMakeStatus"] = "1";
                            TempData["InvoMakeMsg"] = "Invoice saved successfully.Invoice No: " + invoice.InvoiceNo;
                        }

                        else
                        {
                            return Content("<script language='javascript' type='text/javascript'>alert('No Payment has been Set!');</script>");
                        }

                        Session["PaidAmount"] = 0;
                        Session["data"] = null;


                    }


                }
            }
            catch (Exception ex)
            {
                TempData["InvoMakeStatus"] = "2";
                TempData["InvoMakeMsg"] = "Failed To Make Invoice.Try Again.";

                AppLogger.PrintException(ex, "");
            }

            return RedirectToAction("Index", "DmsSalesInvoice");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeInvoice_FromOrder(InvoiceViewModel aInvoiceViewModel, int TranID)
        {

            //int _tranID = TranID;
            try
            {
                int _tranID = TranID;
                using (var ts = new TransactionScope())
                {
                    aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount = 0;
                    int nCustomerId = (int)Session["CustomerId"];
                    var barcodes = aInvoiceViewModel.DmsSalesInvoiceDetail.Select(a => a.BarcodeSerial).ToList();
                    var productStocks = (from ab in _db.DmsProductStockSerials
                                         join b in _db.ProductDetails on ab.ProductId equals b.ProductId
                                         join c in _db.DmsProductStockTran on ab.TranId equals c.TranId
                                         where barcodes.Contains(ab.ProductSerial)
                                         && ab.Status == 1
                                         && c.ToCustomerId == nCustomerId
                                         select new { ab.ProductId, ab.ProductSerial, b.Rsp, b.CostPrice }).ToList();
                    if (productStocks.Count != aInvoiceViewModel.DmsSalesInvoiceDetail.Count)
                    {
                        TempData["InvoMakeStatus"] = "2";
                        TempData["InvoMakeMsg"] = "Please select valid product code";
                    }
                    else
                    {
                        var invoiceBarcode = aInvoiceViewModel.DmsSalesInvoiceDetail.FindAll(a => a.IsFreeProduct != true)
                                .Select(b => b.BarcodeSerial).ToList();
                        var invoiceDiscounts = aInvoiceViewModel.DmsInvoiceDiscount.Where(a => a.Amount > 0).ToList();
                        aInvoiceViewModel.DmsSalesInvoice.DiscountAmount = invoiceDiscounts.Sum(a => a.Amount);
                        foreach (string aBarcode in invoiceBarcode)
                        {
                            aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount += productStocks.First(a => a.ProductSerial == aBarcode).Rsp;
                        }
                        aInvoiceViewModel.DmsSalesInvoice.InvoiceAmount -=
                            aInvoiceViewModel.DmsSalesInvoice.DiscountAmount;
                        int consumerId = InsertConsumerAndGetConsumerId(aInvoiceViewModel.Consumer);
                        var invoice = InsertInvoiceAndGetInvoiceId(consumerId, aInvoiceViewModel.DmsSalesInvoice, aInvoiceViewModel);
                        int tranId = InsertDmsProductStockTranAndGetTrandId(aInvoiceViewModel.DmsSalesInvoice.Remarks,
                            invoice.InvoiceId);
                        InsertInvoiceDetails(invoice.InvoiceId, tranId, aInvoiceViewModel.DmsSalesInvoiceDetail);
                        GetWarranyParamAndInsertWarranty(invoice.InvoiceId);
                        invoiceDiscounts.ForEach(a => a.InvoiceId = invoice.InvoiceId);
                        InsertInvoiceDiscount(invoiceDiscounts);
                        var invoiceProductBarcodes = aInvoiceViewModel.DmsSalesInvoiceDetail
                                                    .Where(a => !a.IsFreeProduct).Select(b => b.BarcodeSerial).ToList();
                        var invoiceProductIds = productStocks.Where(a => invoiceProductBarcodes.Contains(a.ProductSerial))
                                                      .Select(c => c.ProductId).ToList();
                        InsertPromoDetails(invoiceProductIds, invoice.InvoiceId);
                        //PaymentDetails pp = new PaymentDetails();
                        //pp.InvoiceID = invoice.InvoiceId;
                        double Netpayble = invoice.InvoiceAmount;
                        double Paid = Convert.ToDouble(Session["PaidAmount"]);
                        if (Netpayble == Paid)
                        {
                            InsertPaymentDetails(invoice.InvoiceId);
                            Update_Order(_tranID, Netpayble, invoice.InvoiceId);
                            _db.SaveChanges();
                            ts.Complete();
                            TempData["InvoMakeStatus"] = "1";
                            TempData["InvoMakeMsg"] = "Invoice saved successfully.Invoice No: " + invoice.InvoiceNo;
                        }
                        else
                        {
                            return (Content("<script language='javascript' type='text/javascript'>alert('No Payment has been Set!');</script>"));
                        }

                        Session["PaidAmount"] = 0;
                        Session["data"] = null;


                    }


                }
            }
            catch (Exception ex)
            {
                TempData["InvoMakeStatus"] = "2";
                TempData["InvoMakeMsg"] = "Failed To Make Invoice.Try Again.";

                AppLogger.PrintException(ex, "");
            }

            return RedirectToAction("Index", "DmsSalesInvoice");
        }
        [HttpPost]
        public JsonResult PayDetails(List<PaymentDetails> Details)
        {
            Session["data"] = Details.ToList();
            return Json(Details);
        }
        [HttpPost]
        public JsonResult GetRetail(int TranID)
        {
            //   var data = _db.Outlets.Find(RetailID);
            string sGetOutlet = "Select RetailName ,RetailAddress,Mobile01,Email  From t_DMSOrder a, t_DMSCLusterOutlet b  where a.OutletID=b.Retailid and TranID=" + TranID + "";
            var allOutlet = _db.Database.SqlQuery<string>(sGetOutlet).ToList();
            ViewBag.OutletList = new SelectList(allOutlet, "RetailID", "RetailName");
            ViewBag.RetailDetail = allOutlet;

            return Json(allOutlet);
        }
        [NonAction]
        private void InsertInvoiceDiscount(List<DmsInvoiceDiscount> dmsInvoiceDiscounts)
        {
            int maxDiscountId = _db.DmsInvoiceDiscounts.DefaultIfEmpty().Max(r => r == null ? 1 : r.DiscountId + 1);

            foreach (var aDmsInvoiceDiscounts in dmsInvoiceDiscounts)
            {
                aDmsInvoiceDiscounts.DiscountId = maxDiscountId++;
                _db.DmsInvoiceDiscounts.Add(aDmsInvoiceDiscounts);
            }
        }
        [NonAction]
        private void InsertPromoDetails(List<int> productIds, int invoiceId)
        {


            string promoQuery = @"Select * from [t_DMSCurrentPromo] where  ProductId in ({0}) ";
            //string productIdWIthComma = string.Join(",", productIds);
            promoQuery = string.Format(promoQuery, string.Join(",", productIds));
            var promoDetails = _db.Database.SqlQuery<PromoDetailsViewModel>(promoQuery).ToList();
            promoDetails.ForEach(aPromo =>
            {
                var aPromoDetails = new DmsPromoAppliedToInvoice
                {
                    InvoiceId = invoiceId,
                    Amount = aPromo.Discount,
                    PromoId = aPromo.PromoId,
                    PromoType = aPromo.PromoType,
                    ProductId = aPromo.ProductId
                };
                _db.DmsPromoCpAppliedToInvoices.Add(aPromoDetails);
            });
        }
        [NonAction]
        private int InsertDmsProductStockTranAndGetTrandId(string remarks, int invoiceId)
        {
            var tranId = _db.DmsProductStockTran.DefaultIfEmpty().Max(r => r == null ? 1 : r.TranId + 1);
            var tranNo = "DMS-" + _db.DmsProductStockTran.DefaultIfEmpty().Max(r => r == null ? 1 : r.TranId + 1);

            var aDmsProductStockTran = new DmsProductStockTran
            {
                TranId = tranId,
                TranNo = tranNo,
                TranDate = DateTime.Today,
                TranType = (int)ProductStockTranType.Invoice,
                TranSide = (int)ProductStockTranSide.Out,
                FromCustomerId = (int)Session["CustomerId"],
                ToCustomerId = 1,
                Remarks = remarks,
                RefInvoiceId = invoiceId,
                CreateDate = DateTime.Now
            };
            _db.DmsProductStockTran.Add(aDmsProductStockTran);
            return tranId;
        }
        [NonAction]
        private int InsertConsumerAndGetConsumerId(Consumer aConsumer)
        {

            aConsumer.CustomerId = (int)Session["CustomerId"];
            aConsumer.DateofBirth = DateTime.Now;
            aConsumer.ConsumerId = _db.Consumers.DefaultIfEmpty().Max(r => r == null ? 1 : r.ConsumerId + 1);
            if (Session["UserType"].ToString() == "DB")
            {
                try
                {
                    aConsumer.ReffID = Convert.ToInt32(Request.Form["ConsumerId"].ToString());
                }
                catch
                {
                    aConsumer.ReffID = 0;
                }
            }
            else
            {
                aConsumer.ReffID = 0;
            }

            string noOfZeros = "";
            for (int i = aConsumer.ConsumerId.ToString().Length; i < 4; i++)
            {
                noOfZeros += "0";
            }
            aConsumer.ConsumerCode = "DMS-" + noOfZeros + aConsumer.ConsumerId;
            aConsumer.PhoneNo = aConsumer.ContactNo;

            _db.Consumers.Add(aConsumer);

            return aConsumer.ConsumerId;
        }
        private int InsertConsumerAndGetConsumerId_DB()
        {
            Consumer aConsumer = new Consumer();
            var sql = "Select RetailName,RetailAddress,Mobile01, EMail from t_DMSCLusterOutlet where RetailID=" + Request.Form["Outlet"].ToString() + "";
            var promoDetails = _db.Database.SqlQuery<Consumer>(sql).ToList();
            foreach (var Dt in promoDetails)
            {
                aConsumer.ConsumerName = Dt.ConsumerName;
                aConsumer.Address = Dt.Address;
                aConsumer.ContactNo = Dt.ContactNo;
                aConsumer.Email = Dt.Email;
            }
            aConsumer.CustomerId = (int)Session["CustomerId"];
            aConsumer.DateofBirth = DateTime.Now;
            aConsumer.ConsumerId = _db.Consumers.DefaultIfEmpty().Max(r => r == null ? 1 : r.ConsumerId + 1);
            string noOfZeros = "";
            for (int i = aConsumer.ConsumerId.ToString().Length; i < 4; i++)
            {
                noOfZeros += "0";
            }
            aConsumer.ConsumerCode = "DMS-" + noOfZeros + aConsumer.ConsumerId;
            //   aConsumer.PhoneNo = aConsumer.ContactNo;
            _db.Consumers.Add(aConsumer);
            return aConsumer.ConsumerId;
        }
        private void InsertPaymentDetails(int invoiceid)
        {
            PaymentDetails Details = new PaymentDetails();
            List<PaymentDetails> objlt = (List<PaymentDetails>)Session["data"];
            foreach (var List in objlt)
            {
                List.InvoiceID = invoiceid;
                _db.Details.Add(List);
            }

        }
        private void Update_Order(int TranID, double DeliveryAMount, int invoiceID)
        {

            int _TranID = Convert.ToInt32(TranID);
            SecOrder UpdatedOrder = (from c in _db.SecOrder
                                     where c.TranID == _TranID
                                     select c).FirstOrDefault();
            if (UpdatedOrder != null)
            {
                UpdatedOrder.IsDelivered = true;
                UpdatedOrder.DeliveryDate = DateTime.Today;
                UpdatedOrder.MemoNo = invoiceID.ToString();
                UpdatedOrder.DeliveryAmount = Convert.ToDecimal(DeliveryAMount);
            }
            else
            {
                ViewBag.Message = "TranID not found.";
            }


        }
        public JsonResult TotalPaid(string sumVal)
        {
            Session["PaidAmount"] = sumVal;
            return Json(sumVal);
        }

        [NonAction]
        private DmsSalesInvoice InsertInvoiceAndGetInvoiceId(int consumerId,
            DmsSalesInvoice aDmsSalesInvoice,
            InvoiceViewModel aInvoiceViewModel)
        {
            int nCustomerId = (int)Session["CustomerId"];
            aDmsSalesInvoice.InvoiceId =
            _db.DmsSalesInvoices.DefaultIfEmpty().Max(r => r == null ? 1 : r.InvoiceId + 1);
            int numberOfInvoice = _db.DmsSalesInvoices.Count
                (m => m.CustomerId == nCustomerId
                    && m.InvoiceDate.Month == DateTime.Now.Month
                    && m.InvoiceDate.Year == DateTime.Now.Year) + 1;


            aDmsSalesInvoice.InvoiceNo = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                         numberOfInvoice.ToString("000");
            aDmsSalesInvoice.InvoiceDate = DateTime.Today;
            aDmsSalesInvoice.InvoiceTypeId = 1;
            aDmsSalesInvoice.CustomerId = (int)Session["CustomerId"];
            aDmsSalesInvoice.CreateUserId = (int)Session["UserId"];
            aDmsSalesInvoice.CreateDate = DateTime.Now;
            aDmsSalesInvoice.ConsumerId = consumerId;
            aDmsSalesInvoice.Status = 1;
            _db.DmsSalesInvoices.Add(aDmsSalesInvoice);
            _db.SaveChanges();
            return aDmsSalesInvoice;

        }
        [NonAction]
        private void InsertInvoiceDetails(int invoiceId, int tranId, List<DmsSalesInvoiceDetail> dmsSalesInvoiceDetail)
        {
            int nCustomerId = (int)Session["CustomerID"];
            //List<ProductDetails> selectedProducts = (List<ProductDetails>) Session["selectedProducts"];
            int nNoOfProduct = 0;
            //------------------------------------------------------------------------------------
            var invoiceDetailsId = _db.DmsSalesInvoiceDetail
                                    .DefaultIfEmpty().Max(r => r == null ? 1 : r.InvoiceDetailId + 1);
            var barcodes = dmsSalesInvoiceDetail.Select(a => a.BarcodeSerial).ToList();
            var productStocks = (from ab in _db.DmsProductStockSerials
                                 join b in _db.ProductDetails on ab.ProductId equals b.ProductId
                                 join c in _db.DmsProductStockTran on ab.TranId equals c.TranId
                                 where barcodes.Contains(ab.ProductSerial)
                                 && ab.Status == 1 && c.ToCustomerId == nCustomerId
                                 select new { ab.ProductId, ab.ProductSerial, b.Rsp, b.CostPrice }).ToList();


            foreach (var aProductStock in productStocks)
            {
                var aDmsSalesInvoiceDetail = new DmsSalesInvoiceDetail
                {
                    InvoiceDetailId = invoiceDetailsId,
                    InvoiceId = invoiceId,
                    ProductId = aProductStock.ProductId,
                    BarcodeSerial = aProductStock.ProductSerial,
                    UnitPrice = aProductStock.Rsp,
                    CostPrice = aProductStock.CostPrice,
                    FreeQty = 0,
                    IsFreeProduct = dmsSalesInvoiceDetail.First(a => a.BarcodeSerial == aProductStock.ProductSerial).IsFreeProduct
                };
                _db.DmsSalesInvoiceDetail.Add(aDmsSalesInvoiceDetail);
                var aDmsProductStockSerial = _db.DmsProductStockSerials.First(a => a.ProductSerial == aProductStock.ProductSerial
                                                           && a.ProductId == aProductStock.ProductId);
                aDmsProductStockSerial.Status = (int)ProductSerialStatus.Sold;
                _db.DmsProductStockSerials.AddOrUpdate(aDmsProductStockSerial);
                ++invoiceDetailsId;
            }
            var distinctsProducts = productStocks.DistinctBy(a => a.ProductId).Select(b => b.ProductId).ToList();
            foreach (int aProdId in distinctsProducts)
            {
                var aStock = _db.DmsProductStocks.First(a => a.DistributorId == nCustomerId && a.ProductId == aProdId);
                aStock.CurrentStock -= productStocks.FindAll(a => a.ProductId == aProdId).Count;
                _db.DmsProductStocks.AddOrUpdate(aStock);
                var aDmsProductStockTranItem = new DmsProductStockTranItem
                {
                    TranId = tranId,
                    ProductId = aProdId,
                    Qty = productStocks.FindAll(a => a.ProductId == aProdId).Count,
                    CostPrice = productStocks.First(a => a.ProductId == aProdId).CostPrice,
                    SalesPrice = productStocks.First(a => a.ProductId == aProdId).Rsp
                };
                _db.DmsProductStockTranItems.Add(aDmsProductStockTranItem);
            }

            //------------------------------------------------------------------------------------
            //foreach (var aProduct in selectedProducts)
            //{
            //    //Have to be roll back
            //    var invoiceDetailId =_db.DmsSalesInvoiceDetail
            //                        .DefaultIfEmpty().Max(r => r == null ? 1 : r.InvoiceDetailId + 1 + nNoOfProduct);
            //    nNoOfProduct++;
            //    var aDmsSalesInvoiceDetail = new DmsSalesInvoiceDetail
            //    {
            //        InvoiceDetailId = invoiceDetailId,
            //        InvoiceId = invoiceId,
            //        ProductId = aProduct.ProductId,
            //        BarcodeSerial = aProduct.ProductSerial,
            //        UnitPrice = aProduct.Rsp,
            //        CostPrice = aProduct.CostPrice,
            //        FreeQty = 0,
            //        IsFreeProduct = false
            //    };
            //    _db.DmsSalesInvoiceDetail.Add(aDmsSalesInvoiceDetail);
            //    var aDmsProductStock = (from a in _db.DmsProductStocks
            //                            where a.ProductId == aProduct.ProductId
            //                            && a.DistributorId == nCustomerId
            //                            select a).First();
            //   aDmsProductStock.CurrentStock -= 1;
            //    _db.DmsProductStocks.AddOrUpdate(aDmsProductStock);
            //    var aDmsProductStockSerial = (from a in _db.DmsProductStockSerials
            //                                  where a.ProductSerial == aProduct.ProductSerial
            //                                        && a.ProductId == aProduct.ProductId
            //                                  select a).First();
            //    aDmsProductStockSerial.Status = (int)ProductSerialStatus.Sold;
            //    _db.DmsProductStockSerials.AddOrUpdate(aDmsProductStockSerial);
            //}
            //var productIds = selectedProducts.Select(a => a.ProductId).Distinct().ToList();
            //foreach (int aProductId in productIds)
            //{
            //    var aProd = selectedProducts.Find(a => a.ProductId == aProductId);
            //    var aDmsProductStockTranItem = new DmsProductStockTranItem
            //    {
            //        TranId = tranId,
            //        ProductId = aProductId,
            //        Qty = selectedProducts.FindAll(a=>a.ProductId == aProductId).Count,
            //        CostPrice = aProd.CostPrice,
            //        SalesPrice = aProd.Rsp
            //    };
            //    _db.DmsProductStockTranItems.Add(aDmsProductStockTranItem);
            //}
        }
        [NonAction]
        private void GetWarranyParamAndInsertWarranty(int invoiceId)
        {
            List<ProductViewModel> selectedProducts = (List<ProductViewModel>)Session["selectedProducts"];
            int nNoOfWarrantyCard = 0;
            foreach (var aProduct in selectedProducts)
            {
                var warrantycardId = _db.WarrantyCards.DefaultIfEmpty().Max(r => r == null ? 1 : r.WarrantyCardId + 1) + nNoOfWarrantyCard;
                nNoOfWarrantyCard++;
                var warrantyParamId = _db.Database.SqlQuery<int>("Select WarrantyParamID from dbo.t_WarrantyParam where ProductId= " + aProduct.ProductId + " AND IsCurrent=1 ").FirstOrDefault<int>();
                var aWarrantyCard = new WarrantyCard
                {
                    WarrantyCardId = warrantycardId,
                    InvoiceId = invoiceId,
                    ProductSerialNo = aProduct.ProductSerial,
                    WarrantyCardNo = "DMS-" + aProduct.ProductSerial,
                    OutletId = (int)Session["CustomerId"],
                    ProductId = aProduct.ProductId,
                    WarrantyParameterId = warrantyParamId
                };

                _db.WarrantyCards.Add(aWarrantyCard);
            }
        }
        [HttpGet]
        public ActionResult InvoiceReverse()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetInvoices(string invoiceNo)
        {
            try
            {
                var nCustomerId = (int)Session["CustomerID"];
                var invoices = _db.DmsSalesInvoices.Where(a => a.InvoiceNo == invoiceNo
                               && a.CustomerId == nCustomerId && a.Status == 1).
                              Include(a => a.Consumer).
                              ToList();
                var invoiceJson = Json(invoices, JsonRequestBehavior.AllowGet);
                return invoiceJson;
            }
            catch (Exception ex)
            {
                return Json(Responses.Exception(ex), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReverseInvoice(int invoiceId)
        {
            try
            {
                var nCustomerId = (int)Session["CustomerID"];
                var invoice = _db.DmsSalesInvoices.FirstOrDefault(a =>
                 a.InvoiceId == invoiceId && a.CustomerId == nCustomerId);
                if (invoice != null)
                {
                    invoice.Status = 2;
                    invoice.ReverseApplyDate = DateTime.Now;
                    _db.SaveChanges();
                    return Json(Responses.Updated(), JsonRequestBehavior.AllowGet);
                }
                return Json(Responses.Error("Invalid invoice no"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Responses.Exception(ex), JsonRequestBehavior.AllowGet);
            }
        }
    }
}


