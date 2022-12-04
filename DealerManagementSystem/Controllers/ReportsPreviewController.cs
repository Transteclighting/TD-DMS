using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DealerManagementSystem.Models;
using DealerManagementSystem.CrystalReportToPdf;

namespace DealerManagementSystem.Controllers
{
    public class ReportsPreviewController : Controller
    {
        // GET: /ReportsPreview/
        private ProjectDb db = new ProjectDb();

        public ActionResult InvoiceWiseSales()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult ProductWiseSales()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                int nCustomerId = (int)Session["CustomerId"];
                var availableAsg = (from a in db.DmsProductStocks
                                    join b in db.ProductDetails
                                        on a.ProductId equals b.ProductId
                                    where a.DistributorId == nCustomerId
                                    select new { b.AsgId, b.AsgName }).ToList();
                availableAsg = availableAsg.Distinct().ToList();
                ViewBag.availableAsg = new SelectList(availableAsg, "AsgId", "AsgName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult BarcodeWiseSale()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];

                string sGetMag = "Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName, MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                                 " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by MAGSort";
                var allMag = db.ProductGroups.SqlQuery(sGetMag).ToList();

                string sGetBrand = "Select distinct BrandID as PdtGroupId, BrandDesc as PdtGroupName, BrandSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                     " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by BrandSort";
                var allBrand = db.ProductGroups.SqlQuery(sGetBrand).ToList();

                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "PdtGroupId", "PdtGroupName");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult ProductWiseCurrentStock()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];

                string sGetMag = "Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName, MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                                 " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by MAGSort";
                var allMag = db.ProductGroups.SqlQuery(sGetMag).ToList();

                string sGetBrand = "Select distinct BrandID as PdtGroupId, BrandDesc as PdtGroupName, BrandSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                     " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by BrandSort";
                var allBrand = db.ProductGroups.SqlQuery(sGetBrand).ToList();

                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "PdtGroupId", "PdtGroupName");

                return View();
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult BarcodeWiseCurrentStock()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];

                string sGetMag = "Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName, MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                                 " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by MAGSort";
                var allMag = db.ProductGroups.SqlQuery(sGetMag).ToList();

                string sGetBrand = "Select distinct BrandID as PdtGroupId, BrandDesc as PdtGroupName, BrandSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b " +
                     " Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + " Order by BrandSort";
                var allBrand = db.ProductGroups.SqlQuery(sGetBrand).ToList();

                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "PdtGroupId", "PdtGroupName");
                return View();
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult Invoice(int? invoiceId)
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null && invoiceId != null)
            {
                Session["InvoiceId"] = invoiceId;
                return View();
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult WarrantyCard(int? invoiceId)
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null && invoiceId != null)
            {
                var allWarrantyCardId = db.WarrantyCards.Where(a => a.InvoiceId == invoiceId).ToList();
                ViewBag.allWarrantyCardId = allWarrantyCardId;
                return View();
            }
            return RedirectToAction("Login", "User");
        }

        public CrystalReportPdfResult GetInvoicePdf()
        {
            int invoiceId = (int)Session["InvoiceId"];
            Session["InvoiceId"] = null;
            DmsSalesInvoice aDmsSalesInvoice = db.DmsSalesInvoices.First(a => a.InvoiceId == invoiceId);
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptRetailInvoice.rpt");
            string loggedUserName = (string)Session["LoggedUserName"];
            string loggedUserFullName = (string)Session["LoggedUserFullName"];
            return new CrystalReportPdfResult(reportPath, invoiceId, (int)Session["CustomerID"], aDmsSalesInvoice.ConsumerId, loggedUserName, loggedUserFullName);
        }
        public CrystalReportPdfResult GetWarrantyCardPdf(string pSerial)
        {
            string reportPath = "";
            WarrantyCard Serial = db.WarrantyCards.First(a => a.ProductSerialNo == pSerial);
            int ProductID = Serial.ProductId;

            ProductDetails ProdDesc = db.ProductDetails.First(a => a.ProductId == ProductID);
            if (ProdDesc.BrandDesc == "ROWA")
            {
                reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptWarrantyCardRowa.rpt");
            }
            else
            {
                reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptWarrantyCard.rpt");
            }


            if (Session["UserType"].ToString() == "DB")
            {
                return new CrystalReportPdfResult(reportPath, pSerial.Trim(), "DB");

            }
            else
            {
                return new CrystalReportPdfResult(reportPath, pSerial.Trim(), "Others");

            }
        }

        public CrystalReportPdfResult GetInvoiceWiseSalesPdf(string sStartDate, string sEndDate)
        {
            var dtStartDate = DateTime.Parse(sStartDate.Trim()).Date;
            var dtEndDate = DateTime.Parse(sEndDate.Trim()).Date;
            var nCustomerId = (int)Session["CustomerId"];
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptInvoiceWiseSales.rpt");
            return new CrystalReportPdfResult(reportPath, nCustomerId, dtStartDate, dtEndDate);
        }

        public CrystalReportPdfResult GetProductWiseSlaes(string sStartDate, string sEndDate, string productCode, string productName, string asgName)
        {
            var nCustomerId = (int)Session["CustomerId"];
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptProductWiseSales.rpt");
            return new CrystalReportPdfResult(nCustomerId, reportPath, sStartDate.Trim(), sEndDate.Trim(), productCode, productName, asgName);
        }

        public CrystalReportPdfResult GetBarcodeWiseSlaes(string sStartDate, string sEndDate, string sProductCode, string sProductName, string sBrandName, string sPdtGroupName)
        {
            var dtStartDate = DateTime.Parse(sStartDate.Trim()).Date;
            var dtEndDate = DateTime.Parse(sEndDate.Trim()).Date;
            var nCustomerId = (int)Session["CustomerId"];
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptBarcodeWiseSlaes.rpt");
            return new CrystalReportPdfResult(nCustomerId, reportPath, dtStartDate, dtEndDate, sProductCode.Trim(), sProductName.Trim(), sBrandName, sPdtGroupName);
        }

        public ProductWiseCurrentStock GetProductWiseCurrentStock(string sProductCode, string sProductName, string sPdtGroupName, string sBrandName)
        {
            var nCustomerId = (int)Session["CustomerId"];
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptProductWiseCurrentStock.rpt");
            return new ProductWiseCurrentStock(reportPath, nCustomerId, sProductCode, sProductName, sPdtGroupName, sBrandName);
        }

        public BarcodeWiseCurrentStock GetBarcodeWiseCurrentStock(int? pdtGroupId, string pdtGroupName, int? brandId, string brandName, string productCode, string productName)
        {
            var nCustomerId = (int)Session["CustomerId"];
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptBarcodeWiseCurrentStock.rpt");
            return new BarcodeWiseCurrentStock(reportPath, nCustomerId, pdtGroupId, brandId, productCode, productName);

        }

    }
}
