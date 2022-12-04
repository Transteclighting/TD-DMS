using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DealerManagementSystem.Models;
using DealerManagementSystem.Models.ViewModel;

namespace DealerManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();

        public ActionResult SecOrderItemForInvoice(string Serials, string TranID, string rowcount)
        {
            string _Serials = "(" + Serials + ")";
            string barcodes = Serials;
            int tranid = Convert.ToInt32(TranID);
            int Rows = Convert.ToInt32(rowcount);
            if (Session["CustomerID"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];

                string sGetMag = "Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName, MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                                 " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by MAGSort";
                var allMag = _db.ProductGroups.SqlQuery(sGetMag).ToList();

                string sGetBrand = "Select distinct BrandID as PdtGroupId, BrandDesc as PdtGroupName, BrandSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                     " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by BrandSort";
                var allBrand = _db.ProductGroups.SqlQuery(sGetBrand).ToList();

                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "PdtGroupId", "PdtGroupName");

                string count = @"SELECT  count(productserial)
                            from t_DMSProductStockSerial a
                            INNER JOIN  t_DMSProductStockTran b
                            ON a.TranID = b.TranID                           
                            Where a.Status = 1 and  tocustomerID=" + nCustomerId + " and productserial in {0} ";
                count = string.Format(count, _Serials);
                var total = _db.Database.SqlQuery<int>(count).First();

                if (total == Rows)
                {
                    string barcodeWiseCurrentStock = @"SELECT  CAST(1 as bit) IsChecked,MagId,BrandId, a.ProductId,ProductCode,
                    ProductSerial, ProductName,AsgId,AsgName,MAGName, BrandDesc, RSP,CostPrice,ISNULL(DisAmount, 0) PromoDiscount
                    from t_DMSProductStockSerial a
                    INNER JOIN  t_DMSProductStockTran b
                    ON a.TranID = b.TranID
                    INNER JOIN  v_ProductDetails c
                    ON c.ProductId = a.ProductId
                    LEFT OUTER JOIN (Select ProductID,sum(DisAmount) DisAmount 
                    From t_DMSCurrentPromo
                    group by ProductID) v
                    ON v.ProductId = a.ProductId
                    Where a.Status = 1 and toCustomerID=" + nCustomerId + " and productserial in {0} ";
                    barcodeWiseCurrentStock = string.Format(barcodeWiseCurrentStock, _Serials);
                    //= "(" + Serials + ")");
                    var allAvailabeProduct = _db.Database.SqlQuery<ProductViewModel>(barcodeWiseCurrentStock).ToList();
                    List<ProductViewModel> selectedProducts = null;
                    if (allAvailabeProduct.Count != 0)
                    {
                        selectedProducts = allAvailabeProduct.Where(aProduct => aProduct.IsChecked).ToList();
                    }
                    Session["selectedProducts"] = selectedProducts;
                    return RedirectToAction("MakeInvoice_FromOrder", "DmsSalesInvoice", new { TranID = tranid });
                }
                else
                {
                    ViewBag.Duplicate = "Duplicate Serial Not Allowed";
                    return Content("<script language='javascript' type='text/javascript'>alert('Duplicate Serial Found. Please Correct !!');</script>");
                }


            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult ChooseProductsForInvoice()
        {
            if (Session["CustomerID"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];

                string sGetMag = "Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName, MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                                 " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by MAGSort";
                var allMag = _db.ProductGroups.SqlQuery(sGetMag).ToList();

                string sGetBrand = "Select distinct BrandID as PdtGroupId, BrandDesc as PdtGroupName, BrandSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                     " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by BrandSort";
                var allBrand = _db.ProductGroups.SqlQuery(sGetBrand).ToList();

                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "PdtGroupId", "PdtGroupName");

                string barcodeWiseCurrentStock = @"SELECT  CAST(0 as bit) IsChecked,MagId,BrandId, a.ProductId,ProductCode,
                            ProductSerial, ProductName,AsgId,AsgName,MAGName, BrandDesc, RSP,CostPrice,ISNULL(DisAmount, 0) PromoDiscount
                            from t_DMSProductStockSerial a
                            INNER JOIN  t_DMSProductStockTran b
                            ON a.TranID = b.TranID
                            INNER JOIN  v_ProductDetails c
                            ON c.ProductId = a.ProductId
                            LEFT OUTER JOIN (Select ProductID,sum(DisAmount) DisAmount 
                            From t_DMSCurrentPromo
                            group by ProductID) v
                            ON v.ProductId = a.ProductId
                            Where a.Status = 1 and  productserial in ('11264519','11227826','11113530','11051390','11078190') ";
                barcodeWiseCurrentStock = string.Format(barcodeWiseCurrentStock, nCustomerId);


                var allAvailabeProduct = _db.Database.SqlQuery<ProductViewModel>(barcodeWiseCurrentStock).ToList();

                return View(new List<ProductViewModel>());

            }
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public ActionResult GetBarcode(List<Barcodes> Details)
        {
            return RedirectToAction("ChooseProductsForInvoice", "Product");

        }
        public JsonResult GetProducts(string pdtGroupId, string brandId, string barcodeSlId, string productCodeId, string productNameId)
        {
            var nCustomerId = (int)Session["CustomerID"];

            string barcodeWiseCurrentStock = @"SELECT  CAST(0 as bit) IsChecked,MagId,BrandId, a.ProductId,ProductCode,
                            ProductSerial, ProductName,AsgId,AsgName,MAGName, BrandDesc, RSP,CostPrice,ISNULL(DisAmount, 0) PromoDiscount
                            from t_DMSProductStockSerial a
                            INNER JOIN  t_DMSProductStockTran b
                            ON a.TranID = b.TranID
                            INNER JOIN  v_ProductDetails c
                            ON c.ProductId = a.ProductId
                            LEFT OUTER JOIN (Select ProductID,sum(DisAmount) DisAmount 
                            From t_DMSCurrentPromo
                            group by ProductID) v
                            ON v.ProductId = a.ProductId
                            Where a.Status = 1 and ToCustomerID = {0} ";
            barcodeWiseCurrentStock = string.Format(barcodeWiseCurrentStock, nCustomerId);


            if (pdtGroupId != string.Empty)
            {
                int nMagId = Convert.ToInt32(pdtGroupId);
                // query += " AND MAGID= '" + pdtGroupId + "' ";
                barcodeWiseCurrentStock += " AND MagId = " + nMagId;
            }
            if (brandId != string.Empty)
            {
                int nBrandId = Convert.ToInt32(brandId);

                barcodeWiseCurrentStock += " AND BrandId = " + nBrandId;

            }
            if (barcodeSlId != string.Empty)
            {
                barcodeWiseCurrentStock += " AND ProductSerial = '" + barcodeSlId.Trim() + "' ";
            }

            if (productCodeId != string.Empty)
            {
                //query += " AND ProductCode LIKE '%"+ productCodeId.Trim() + "%' ";
                barcodeWiseCurrentStock += " AND ProductCode LIKE '%" + productCodeId.Trim() + "%' ";
            }
            if (productNameId != string.Empty)
            {
                //query += " AND ProductName LIKE '%" + productNameId.Trim() + "%' ";
                barcodeWiseCurrentStock += " AND ProductName LIKE '%" + productNameId.Trim() + "%' ";
            }
            var allAvailabeProduct = _db.Database.SqlQuery<ProductViewModel>(barcodeWiseCurrentStock).ToList();
            foreach (var item in allAvailabeProduct)
            {
                var query = @"Select IsNull(RSP,0) as RSP
                from t_Finishedgoodsprice a, 
                (Select Max(PriceID)as PriceID 
                from t_finishedgoodsprice where EffectiveDate <=cast(getdate() as date)
                and ProductID = {0})b 
                where a.PriceID=b.PriceID";
                query = string.Format(query, item.ProductId);
                item.Rsp = _db.Database.SqlQuery<double>(query).FirstOrDefault();
            }

            //var allAvailabeProduct = db.ProductDetails.SqlQuery(query).ToList();
            return Json(allAvailabeProduct, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseProductsForInvoice(List<ProductViewModel> products)
        {
            List<ProductViewModel> selectedProducts = null;
            if (products.Count != 0)
            {
                selectedProducts = products.Where(aProduct => aProduct.IsChecked).ToList();
            }

            Session["selectedProducts"] = selectedProducts;
            return RedirectToAction("MakeInvoice", "DmsSalesInvoice");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecOrderItemForInvoice(List<ProductViewModel> products)
        {

            List<ProductViewModel> selectedProducts = null;
            if (products.Count != 0)
            {
                selectedProducts = products.Where(aProduct => aProduct.IsChecked).ToList();
            }

            Session["selectedProducts"] = selectedProducts;
            return RedirectToAction("MakeInvoice", "DmsSalesInvoice");
        }

        [HttpGet]
        public JsonResult GetProductsJson(string productCode, string productName, int? brandId, int? magId)
        {
            string query = "SELECT * FROM t_Product WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(productCode))
            {
                query += " AND ProductCode = '" + productCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query += " AND ProductName LIKE '%" + productName + "%'";
            }

            if (brandId > 0)
            {
                query += " AND BrandId = " + brandId;
            }
            if (magId > 0)
            {
                query += " AND ProductGroupId = " + magId;
            }

            var products = _db.Database.SqlQuery<Product>(query).ToList();
            return Json(products, JsonRequestBehavior.AllowGet);

        }



    }
}
