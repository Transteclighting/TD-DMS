using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DealerManagementSystem.Models;

namespace DealerManagementSystem.CrystalReportToPdf
{
    public class ProductWiseCurrentStock : ActionResult
    {
        private readonly ProjectDb db = new ProjectDb();
        private readonly byte[] _contentBytes;

        public ProductWiseCurrentStock(string reportPath, int nCustomerId, string sProductCode, string sProductName, string sPdtGroupName, string sBrandName)
        {
            var anOutlet = db.DmsOutlet.FirstOrDefault(a => a.CustomerId == nCustomerId);
            var productWiseCurrntStocks = (from a in db.DmsProductStocks
                                           join b in db.ProductDetails on a.ProductId equals b.ProductId
                                           where a.DistributorId == nCustomerId
                                           select new { a.ProductId, a.CurrentStock, b.ProductName, b.MagName, b.AsgName, b.ProductCode, b.BrandDesc, b.Rsp }).ToList();

            if (sProductCode != string.Empty)
            {
                productWiseCurrntStocks =
                    productWiseCurrntStocks.Where(a => a.ProductCode.ToLower().Contains(sProductCode.ToLower()))
                        .ToList();
            }
            if (sProductName != string.Empty)
            {
                productWiseCurrntStocks =
                             productWiseCurrntStocks.Where(a => a.ProductName.ToLower().Contains(sProductName.ToLower()))
                             .ToList();
            }
            if (sPdtGroupName != string.Empty)
            {
                productWiseCurrntStocks =
                     productWiseCurrntStocks.Where(a => a.MagName.ToLower().Contains(sPdtGroupName.ToLower()))
                     .ToList();
            }
            if (sBrandName != string.Empty)
            {
                productWiseCurrntStocks =
                 productWiseCurrntStocks.Where(a => a.BrandDesc.ToLower().Contains(sBrandName.ToLower()))
               .ToList();
            }


            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDataSource(productWiseCurrntStocks);

            reportDocument.SetParameterValue("Outlet", anOutlet.OutletName);
            reportDocument.SetParameterValue("Mobile", anOutlet.ContactNo);
            reportDocument.SetParameterValue("Address", anOutlet.Address);
            reportDocument.SetParameterValue("Email", anOutlet.Email != string.Empty ? anOutlet.OutletName : "");
            
            reportDocument.SetParameterValue("ProductCode", sProductCode);
            reportDocument.SetParameterValue("ProductName", sProductName);
            reportDocument.SetParameterValue("MAGName", sPdtGroupName);
            reportDocument.SetParameterValue("BrandName", sBrandName);

            _contentBytes = ConvertData.StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));

        }
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.ApplicationInstance.Response;
            response.Clear();
            response.Buffer = false;
            response.ClearContent();
            response.ClearHeaders();
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.ContentType = "application/pdf";
            using (var stream = new MemoryStream(_contentBytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }
    }
}