using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DealerManagementSystem.Models;
using DealerManagementSystem.Enums;


namespace DealerManagementSystem.CrystalReportToPdf
{
    public class BarcodeWiseCurrentStock:ActionResult
    {
        private readonly ProjectDb db = new ProjectDb();
        private readonly byte[] _contentBytes;

        public BarcodeWiseCurrentStock(string reportPath, int nCustomerId, int? pdtGroupId,int? brandId,string productCode, string productName)
        {
            var anOutlet = db.DmsOutlet.FirstOrDefault(a => a.CustomerId == nCustomerId);
            
            var barcodeWiseCurrentStock = (from a in db.DmsProductStockSerials
                join b in db.ProductDetails on a.ProductId equals b.ProductId
                join c in db.DmsProductStockTran on a.TranId equals c.TranId
                where c.ToCustomerId == nCustomerId
                && a.Status == (int)ProductSerialStatus.Unsold
                select new {b.MagId,b.BrandId,b.ProductCode, b.ProductName, b.BrandDesc, b.AsgName, b.Rsp, a.ProductSerial}).ToList();

            string sMagName = "MAG-ALL";
            if (pdtGroupId != null)
            {
                barcodeWiseCurrentStock = (from a in barcodeWiseCurrentStock
                    where a.MagId == pdtGroupId
                    select a).ToList();
                sMagName = (from a in db.ProductDetails
                    where a.MagId == pdtGroupId
                    select a.MagName).First();


            }
            string sBrandName = "Brand-ALL";
            if (brandId != null)
            {
                barcodeWiseCurrentStock = (from a in barcodeWiseCurrentStock
                                           where a.BrandId == brandId
                                           select a).ToList();
                sBrandName = (from a in db.ProductDetails
                    where a.BrandId == brandId
                              select a.BrandDesc).First();

            }
            if (productCode != string.Empty)
            {
                barcodeWiseCurrentStock = (from a in barcodeWiseCurrentStock
                    where a.ProductCode==productCode
                    select a).ToList();
            }
            if (productName != string.Empty)
            {
                barcodeWiseCurrentStock = (from a in barcodeWiseCurrentStock
                                           where a.ProductName.ToLower().Contains(productName.ToLower())
                                           select a).ToList();
            }
            

            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDataSource(barcodeWiseCurrentStock);

            reportDocument.SetParameterValue("Outlet", anOutlet.OutletName);
            reportDocument.SetParameterValue("Mobile", anOutlet.ContactNo);
            reportDocument.SetParameterValue("Address", anOutlet.Address);
            reportDocument.SetParameterValue("Email", anOutlet.Email != string.Empty ? anOutlet.OutletName : "");


            reportDocument.SetParameterValue("MagName", sMagName);
            reportDocument.SetParameterValue("BrandName", sBrandName);
            reportDocument.SetParameterValue("ProductCode", productCode);
            reportDocument.SetParameterValue("ProductName", productName??string.Empty);

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