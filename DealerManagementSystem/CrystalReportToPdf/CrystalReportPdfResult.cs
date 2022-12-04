using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DealerManagementSystem.App_Data;
using DealerManagementSystem.Models;
using CJ.QRCode.Codec;
using System.Drawing;
using DealerManagementSystem.App_Start;
using DealerManagementSystem.Models.Helper;
using Microsoft.Ajax.Utilities;


namespace DealerManagementSystem.CrystalReportToPdf
{
    public class CrystalReportPdfResult : ActionResult
    {
        readonly ProjectDb _db = new ProjectDb();
        readonly Library _library = new Library();
        Image _iImage;

        private readonly byte[] _contentBytes;

        //Get Barcode Wise Sales
        public CrystalReportPdfResult(int nCustomerId, string reportPath, DateTime dtStartDate, DateTime dtEndDate, string sProductCode, string sProductName, string sBrandName, string sPdtGroupName)
        {

            var barCodeWiseSales = (from a in _db.DmsSalesInvoiceDetail
                                    join b in _db.DmsSalesInvoices on a.InvoiceId equals b.InvoiceId
                                    join c in _db.ProductDetails on a.ProductId equals c.ProductId
                                    where b.CustomerId == nCustomerId
                                    select new { c.ProductCode, c.ProductName, c.AsgName, c.BrandDesc, c.MagName, b.InvoiceNo, b.InvoiceDate, a.BarcodeSerial }).ToList();
            barCodeWiseSales = barCodeWiseSales.Where(a => a.InvoiceDate >= dtStartDate && a.InvoiceDate <= dtEndDate).ToList();

            if (sProductCode != string.Empty)
            {
                barCodeWiseSales = barCodeWiseSales.Where(p => p.ProductCode.ToLower().Contains(sProductCode.ToLower())).ToList();
            }
            if (sProductName != string.Empty)
            {
                barCodeWiseSales = barCodeWiseSales.Where(p => p.ProductName.ToLower().Contains(sProductName.ToLower())).ToList();
            }
            if (sBrandName != string.Empty)
            {
                barCodeWiseSales = barCodeWiseSales.Where(p => p.BrandDesc.ToLower().Contains(sBrandName.ToLower())).ToList();
            }

            if (sPdtGroupName != string.Empty)
            {
                barCodeWiseSales = barCodeWiseSales.Where(p => p.MagName.ToLower().Contains(sPdtGroupName.ToLower())).ToList();
            }


            var anOutlet = _db.DmsOutlet.FirstOrDefault(a => a.CustomerId == nCustomerId);
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDataSource(barCodeWiseSales);

            reportDocument.SetParameterValue("Outlet", anOutlet != null && anOutlet.OutletName != string.Empty ? anOutlet.OutletName : string.Empty);
            reportDocument.SetParameterValue("Mobile", anOutlet != null && anOutlet.ContactNo != string.Empty ? anOutlet.ContactNo : string.Empty);
            reportDocument.SetParameterValue("Address", anOutlet != null && anOutlet.Address != string.Empty ? anOutlet.Address : string.Empty);
            reportDocument.SetParameterValue("Email", anOutlet != null && anOutlet.Email != string.Empty ? anOutlet.Email : string.Empty);

            reportDocument.SetParameterValue("From", dtStartDate);
            reportDocument.SetParameterValue("To", dtEndDate);
            reportDocument.SetParameterValue("ProductCode", sProductCode);
            reportDocument.SetParameterValue("ProductName", sProductName);
            reportDocument.SetParameterValue("BrandName", sBrandName);
            reportDocument.SetParameterValue("ProductGroupName", sPdtGroupName);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
        }



        //Get Product Wise Sales
        public CrystalReportPdfResult(int nCustomerId, string reportPath, string sStartDate, string sEndDate, string productCode, string productName, string asgName)
        {
            //DateTime dtStartDate, DateTime dtEndDate,AND InvoiceDate >='" + dtStartDate + "' AND InvoiceDate <='" + dtEndDate + "'
            var anOutlet = _db.DmsOutlet.FirstOrDefault(a => a.CustomerId == nCustomerId);
            var query = @"Select ProductId,ProductName,ProductCode,AsgId,
            ASGName,sum(SalesQuantity) SalesQuantity
            From
            (
            Select b.ProductId, ProductName, ProductCode, AsgId, ASGName,
            case when invoiceTypeID = 1 then 1 else -1 end as SalesQuantity
            from t_DMSSalesInvoice a
            join t_DMSSalesInvoiceDetail b on a.InvoiceID = b.InvoiceID
            join v_ProductDetails c on b.ProductID = c.ProductID
            where CustomerID = " + nCustomerId + @" AND InvoiceDate between '" + sStartDate + @"'
            AND '" + sEndDate + @"' and  InvoiceDate <= '" + sEndDate + @"'
            ) x group by ProductId, ProductName, ProductCode, AsgId, ASGName";


            var productWiseSales = _db.Database.SqlQuery<ProductWiseSales>(query).ToList();
            if (productCode != string.Empty)
            {
                productWiseSales = productWiseSales.Where(a => a.ProductCode.ToLower().Contains(productCode.ToLower().ToLower())).ToList();
            }
            if (productName != string.Empty)
            {
                productWiseSales = productWiseSales.Where(a => a.ProductName.ToLower().Contains(productName.Trim())).ToList();
            }
            if (asgName != string.Empty)
            {
                productWiseSales = productWiseSales.Where(a => a.AsgName.ToLower().Contains(asgName.ToLower())).ToList();
            }

            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDataSource(productWiseSales);

            //reportDocument.SetParameterValue("Outlet", anOutlet.OutletName);
            //reportDocument.SetParameterValue("Mobile", anOutlet.ContactNo);
            //reportDocument.SetParameterValue("Address", anOutlet.Address);
            //reportDocument.SetParameterValue("Email", anOutlet.Email != string.Empty ? anOutlet.OutletName : "");
            reportDocument.SetParameterValue("Outlet", anOutlet != null && anOutlet.OutletName != string.Empty ? anOutlet.OutletName : string.Empty);
            reportDocument.SetParameterValue("Mobile", anOutlet != null && anOutlet.ContactNo != string.Empty ? anOutlet.ContactNo : string.Empty);
            reportDocument.SetParameterValue("Address", anOutlet != null && anOutlet.Address != string.Empty ? anOutlet.Address : string.Empty);
            reportDocument.SetParameterValue("Email", anOutlet != null && anOutlet.Email != string.Empty ? anOutlet.Email : string.Empty);

            reportDocument.SetParameterValue("From", sStartDate);
            reportDocument.SetParameterValue("To", sEndDate);
            reportDocument.SetParameterValue("ProductCode", productCode != string.Empty ? productCode : "<All>");
            reportDocument.SetParameterValue("ProductName", productName != string.Empty ? productName : "<All>");
            reportDocument.SetParameterValue("AsgName", asgName != string.Empty ? asgName : "<All>");

            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));

        }

        //Get Invoice Wise Sales
        public CrystalReportPdfResult(string reportPath, int nCustomerId, DateTime dtStartDate, DateTime dtEndDate)
        {
            var anOutlet = _db.DmsOutlet.FirstOrDefault(a => a.CustomerId == nCustomerId);
            var invoices = (from a in _db.DmsSalesInvoices
                            join b in _db.Consumers
                            on a.ConsumerId equals b.ConsumerId
                            where a.CustomerId == nCustomerId && a.InvoiceDate >= dtStartDate && a.InvoiceDate <= dtEndDate
                            select new { a.InvoiceNo, a.InvoiceDate, CustomerName = b.ConsumerName, GrossAmount = a.DiscountAmount + a.InvoiceAmount, a.DiscountAmount, NetAmount = a.InvoiceAmount }).ToList();

            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDataSource(invoices);

            //reportDocument.SetParameterValue("Outlet", anOutlet.OutletName);
            //reportDocument.SetParameterValue("Mobile", anOutlet.ContactNo);
            //reportDocument.SetParameterValue("Address", anOutlet.Address);
            //reportDocument.SetParameterValue("Email", anOutlet.Email != string.Empty ? anOutlet.OutletName : "");

            reportDocument.SetParameterValue("Outlet", anOutlet != null && anOutlet.OutletName != string.Empty ? anOutlet.OutletName : string.Empty);
            reportDocument.SetParameterValue("Mobile", anOutlet != null && anOutlet.ContactNo != string.Empty ? anOutlet.ContactNo : string.Empty);
            reportDocument.SetParameterValue("Address", anOutlet != null && anOutlet.Address != string.Empty ? anOutlet.Address : string.Empty);
            reportDocument.SetParameterValue("Email", anOutlet != null && anOutlet.Email != string.Empty ? anOutlet.Email : string.Empty);
            reportDocument.SetParameterValue("From", dtStartDate);
            reportDocument.SetParameterValue("To", dtEndDate);
            // }
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
        }

        //Get Warranty Card
        public CrystalReportPdfResult(string reportPath, string pSerial, string UserType)
        {
            try
            {
                WarrantyCard aWarrantyCard = _db.WarrantyCards.First(a => a.ProductSerialNo == pSerial);

                string query = "Select CAST(0 as bit) IsChecked,BrandId,MagId, ProductId,ProductCode,'000' as ProductSerial, ProductCode, ProductName, AsgId,AsgName,MAGName, BrandDesc, RSP,CostPrice "
                               + "FROM v_ProductDetails  Where ProductId = " + aWarrantyCard.ProductId + " ";
                var aProductDetail = _db.ProductDetails.SqlQuery(query).First();

                var anInvoice = _db.DmsSalesInvoices.FirstOrDefault(a => a.InvoiceId == aWarrantyCard.InvoiceId);
                Consumer aConsumer = _db.Consumers.First(a => a.ConsumerId == anInvoice.ConsumerId);

                var anOutlet = _db.DmsOutlet.FirstOrDefault(a => a.CustomerId == anInvoice.CustomerId);
                var aDmsUser = _db.DmsUsers.FirstOrDefault(a => a.UserId == anInvoice.CreateUserId);
                var aWarrantyParam =
                    _db.WarrantyParams.FirstOrDefault(a => a.WarrantyParamId == aWarrantyCard.WarrantyParameterId);

                QrCodeGen(aConsumer, aProductDetail, anInvoice, aWarrantyParam, pSerial);
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(reportPath);
                DSQRCode oDSQRCode = new DSQRCode();
                byte[] pImage = imageToByteArray(_iImage);
                oDSQRCode.QRCode.AddQRCodeRow(pImage);
                oDSQRCode.AcceptChanges();
                reportDocument.SetDataSource(oDSQRCode);

                reportDocument.SetParameterValue("WarrantyCardNo", aWarrantyCard.WarrantyCardNo);

                reportDocument.SetParameterValue("Service", aWarrantyParam.ServiceWarranty);
                reportDocument.SetParameterValue("Spare", aWarrantyParam.PartsWarranty);
                reportDocument.SetParameterValue("Special", aWarrantyParam.SpecialComponentWarranty);

                if (UserType == "DB")
                {
                    reportDocument.SetParameterValue("Name", "");
                    reportDocument.SetParameterValue("Address", "");
                    reportDocument.SetParameterValue("Telephone", "");
                    reportDocument.SetParameterValue("Mobile", "");
                    reportDocument.SetParameterValue("Email", "");
                    reportDocument.SetParameterValue("DealerName", "");
                    reportDocument.SetParameterValue("IsDealer", false);
                    reportDocument.SetParameterValue("InvoiceDate", "");
                    reportDocument.SetParameterValue("OutletName", aConsumer.ConsumerName);
                    reportDocument.SetParameterValue("InvoiceNo", "");
                }
                else
                {
                    reportDocument.SetParameterValue("Name", aConsumer.ConsumerName);
                    reportDocument.SetParameterValue("Address", aConsumer.Address);
                    reportDocument.SetParameterValue("Telephone", aConsumer.PhoneNo);
                    reportDocument.SetParameterValue("Mobile", aConsumer.ContactNo);
                    reportDocument.SetParameterValue("Email", aConsumer.Email);
                    reportDocument.SetParameterValue("DealerName", "");
                    reportDocument.SetParameterValue("IsDealer", true);
                    reportDocument.SetParameterValue("InvoiceNo", anInvoice.InvoiceNo);
                    reportDocument.SetParameterValue("InvoiceDate", anInvoice.InvoiceDate.ToShortDateString());
                    reportDocument.SetParameterValue("OutletName", anOutlet != null && anOutlet.OutletName != string.Empty ? anOutlet.OutletName : string.Empty);
                }


                //reportDocument.SetParameterValue("InvoiceDate", Convert.ToDateTime(_oSalesInvoice.InvoiceDate).ToString("dd-MMM-yyyy"));


                reportDocument.SetParameterValue("ProductName", aProductDetail.ProductName);
                reportDocument.SetParameterValue("BrandName", aProductDetail.BrandDesc);
                reportDocument.SetParameterValue("ProductCode", aProductDetail.ProductCode);
                reportDocument.SetParameterValue("ExtendedWarranty", "");
                reportDocument.SetParameterValue("SpecialComponent", "Motherboard");
                // reportDocument.SetParameterValue("InvoiceNo", anInvoice.InvoiceNo);
                //reportDocument.SetParameterValue("OutletName",anOutlet.OutletName);
                //reportDocument.SetParameterValue("OutletName", anOutlet != null && anOutlet.OutletName != string.Empty ? anOutlet.OutletName : string.Empty);

                reportDocument.SetParameterValue("Employee", aDmsUser != null && aDmsUser.UserFullName != string.Empty ? aDmsUser.UserFullName : string.Empty);
                reportDocument.SetParameterValue("Barcode", pSerial);

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            }
            catch (Exception ex)
            {
                AppLogger.PrintException(ex, "");
                throw;
            }



        }
        public CrystalReportPdfResult(string reportPath, int invoiceId, int customerId, int consumerId, string loggedUserName, string loggedUserFullName)
        {
            try
            {
                DmsOutlet aDmsOutlet = _db.DmsOutlet.FirstOrDefault(a => a.CustomerId == customerId);
                Consumer aConsumer = _db.Consumers.FirstOrDefault(a => a.ConsumerId == consumerId);
                DmsSalesInvoice aDmsSalesInvoice = _db.DmsSalesInvoices.FirstOrDefault(a => a.InvoiceId == invoiceId);
                var dmsSalesInvoiceDetails =
                    _db.DmsSalesInvoiceDetail.Where(a => a.InvoiceId == invoiceId);

                string barcodes = "";

                var productList = dmsSalesInvoiceDetails.DistinctBy(a => a.ProductId).Select(b => b.ProductId).ToList();
                foreach (var aProductId in productList)
                {
                    var anInvoiceDetails = dmsSalesInvoiceDetails.Where(a => a.ProductId == aProductId).ToList();
                    var aProduct = _db.Products.First(a => a.ProductId == aProductId);
                    barcodes += "<" + aProduct.ProductCode + ">";

                    foreach (var aDetails in anInvoiceDetails)
                    {

                        barcodes += aDetails.BarcodeSerial;
                        if (anInvoiceDetails.IndexOf(aDetails) != anInvoiceDetails.Count - 1)
                        {
                            barcodes += ",";
                        }
                    }
                }


                string discountDetails = string.Empty;

                var invoiceDiscounts = (from p in _db.DmsInvoiceDiscounts
                                        join q in _db.DmsDiscountTypes on p.DiscountTypeId equals q.DiscountTypeId
                                        where p.Amount > 0 && p.InvoiceId == aDmsSalesInvoice.InvoiceId
                                        select new { p.InvoiceId, q.DiscountTypeName, p.Amount }).ToList();

                foreach (var aDiscount in invoiceDiscounts)
                {
                    discountDetails += aDiscount.DiscountTypeName + " : " + aDiscount.Amount;
                    if (invoiceDiscounts.IndexOf(aDiscount) != invoiceDiscounts.Count - 1)
                    {
                        discountDetails += " | ";
                    }
                }


                /*  var invoicedItems = (from a in _db.DmsSalesInvoiceDetail
                                       join b in _db.Products on a.ProductId equals b.ProductId
                                       where (a.InvoiceId == invoiceId && a.IsFreeProduct != true)
                                       select new { b.ProductCode, b.ProductName, a.UnitPrice, Qty = 1 }).ToList();*/

                var invoicedItems = (from p in _db.DmsSalesInvoiceDetail
                                     join q in _db.Products on p.ProductId equals q.ProductId
                                     where p.InvoiceId == aDmsSalesInvoice.InvoiceId && p.IsFreeProduct != true
                                     group p by new { q.ProductId, q.ProductCode, q.ProductName } into g
                                     select new { g.Key.ProductId, g.Key.ProductCode, g.Key.ProductName, Qty = g.Count() }).ToList();






                var freeItems = (from p in _db.DmsSalesInvoiceDetail
                                 join q in _db.Products on p.ProductId equals q.ProductId
                                 where p.InvoiceId == aDmsSalesInvoice.InvoiceId && p.IsFreeProduct
                                 group p by new { p.InvoiceId, q.ProductId, q.ProductCode, q.ProductName } into g
                                 select new { g.Key.InvoiceId, g.Key.ProductCode, g.Key.ProductName, Qty = g.Count() }).ToList();




                List<CreditCard> creditCards = new List<CreditCard>
            {
                new CreditCard
                {
                    BankName = "Bank Asia",
                    Amount = 6999,
                    CardNo = "123456",
                    CardType = "Visa",
                    InstallmentNo = 2,
                    IsEMI = "Yes",
                    POSMachine = "Pos"
                },
                new CreditCard
                {
                    BankName = "Dutch Bangla Bank",
                    Amount = 6999,
                    CardNo = "123456",
                    CardType = "Master Card",
                    InstallmentNo =5,
                    IsEMI = "Yes",
                    POSMachine = "Pos"
                }
            };

                DSSalesInvoiceDetail dsSalesInvoice = new DSSalesInvoiceDetail();
                DSSalesInvoiceDetail dsSalesProducts = new DSSalesInvoiceDetail();
                DSSalesInvoiceDetail dsFreeGiftProduct = new DSSalesInvoiceDetail();
                DSSalesInvoiceDetail dsCreditCard = new DSSalesInvoiceDetail();

                foreach (var aProduct in invoicedItems)
                {
                    DSSalesInvoiceDetail.SalesProductRow oSalesProductRow = dsSalesProducts.SalesProduct.NewSalesProductRow();
                    oSalesProductRow.ProductCode = aProduct.ProductCode;
                    oSalesProductRow.ProductName = aProduct.ProductName;
                    oSalesProductRow.UnitPrice =
                        dmsSalesInvoiceDetails.First(a => a.ProductId == aProduct.ProductId).UnitPrice;
                    oSalesProductRow.Qty = aProduct.Qty;

                    dsSalesProducts.SalesProduct.AddSalesProductRow(oSalesProductRow);
                    dsSalesProducts.AcceptChanges();
                }

                foreach (var aProduct in freeItems)
                {
                    DSSalesInvoiceDetail.FreeGiftProductRow oFreeGiftProductRow = dsFreeGiftProduct.FreeGiftProduct.NewFreeGiftProductRow();
                    oFreeGiftProductRow.ProductCode = aProduct.ProductCode;
                    oFreeGiftProductRow.ProductName = aProduct.ProductName;
                    oFreeGiftProductRow.Qty = aProduct.Qty;

                    dsFreeGiftProduct.FreeGiftProduct.AddFreeGiftProductRow(oFreeGiftProductRow);
                    dsFreeGiftProduct.AcceptChanges();
                }

                foreach (var aCreditCards in creditCards)
                {
                    DSSalesInvoiceDetail.CreditCardInfoRow oCreditCardInfoRow =
                    dsCreditCard.CreditCardInfo.NewCreditCardInfoRow();
                    oCreditCardInfoRow.BankName = aCreditCards.BankName;
                    oCreditCardInfoRow.Amount = aCreditCards.Amount;
                    oCreditCardInfoRow.CardNo = aCreditCards.CardNo;
                    oCreditCardInfoRow.CardType = aCreditCards.CardType;
                    oCreditCardInfoRow.InstallmentNo = aCreditCards.InstallmentNo;
                    oCreditCardInfoRow.IsEMI = aCreditCards.IsEMI;
                    oCreditCardInfoRow.POSMachine = aCreditCards.POSMachine;

                    dsCreditCard.CreditCardInfo.AddCreditCardInfoRow(oCreditCardInfoRow);
                    dsCreditCard.AcceptChanges();

                }
                dsSalesInvoice.Merge(dsSalesProducts);
                dsSalesInvoice.Merge(dsFreeGiftProduct);
                dsSalesInvoice.Merge(dsCreditCard);
                dsSalesInvoice.AcceptChanges();

                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(reportPath);
                reportDocument.SetDataSource(dsSalesInvoice);

                //reportDocument.SetParameterValue("Outlet", aDmsOutlet.OutletName);
                //reportDocument.SetParameterValue("Address", aDmsOutlet.Address);
                //reportDocument.SetParameterValue("Mobile", aDmsOutlet.ContactNo + " " + aDmsOutlet.Email);
                reportDocument.SetParameterValue("Outlet", aDmsOutlet != null && aDmsOutlet.OutletName != string.Empty ? aDmsOutlet.OutletName : string.Empty);
                reportDocument.SetParameterValue("Address", aDmsOutlet != null && aDmsOutlet.Address != string.Empty ? aDmsOutlet.Address : string.Empty);
                reportDocument.SetParameterValue("Mobile", aDmsOutlet != null && aDmsOutlet.ContactNo != string.Empty ? aDmsOutlet.ContactNo : string.Empty);

                reportDocument.SetParameterValue("HC", "16212 or 09613700700, e-mail:homecare@transcombd.com");

                reportDocument.SetParameterValue("CustomerCode", aConsumer.ConsumerCode);
                reportDocument.SetParameterValue("CustomerName", aConsumer.ConsumerName);
                reportDocument.SetParameterValue("CustomerEmail", aConsumer.Email);
                reportDocument.SetParameterValue("CustomerAddress", aConsumer.Address);
                reportDocument.SetParameterValue("CustomerCellNo", aConsumer.ContactNo);
                reportDocument.SetParameterValue("CustomerPhoneNo", aConsumer.PhoneNo);


                reportDocument.SetParameterValue("InvoiceNo", aDmsSalesInvoice.InvoiceNo);
                reportDocument.SetParameterValue("InvoiceDate", aDmsSalesInvoice.InvoiceDate.ToShortDateString());
                reportDocument.SetParameterValue("W/H", aDmsOutlet.OutletName);
                reportDocument.SetParameterValue("DeliveryW/H", aDmsOutlet.OutletName);


                reportDocument.SetParameterValue("RefInvoice", "No");

                reportDocument.SetParameterValue("SPDiscount", aDmsSalesInvoice.DiscountAmount);
                reportDocument.SetParameterValue("smsDiscount", 0);
                reportDocument.SetParameterValue("SDiscount", 0);
                reportDocument.SetParameterValue("Discount", aDmsSalesInvoice.DiscountAmount);

                reportDocument.SetParameterValue("FCharge", 0);
                reportDocument.SetParameterValue("ICharge", 0);
                reportDocument.SetParameterValue("OCharge", 0);
                reportDocument.SetParameterValue("MCharge", 0);
                reportDocument.SetParameterValue("Charge", 0);


                reportDocument.SetParameterValue("Cash", aDmsSalesInvoice.DiscountAmount);
                reportDocument.SetParameterValue("Credit", 0);
                reportDocument.SetParameterValue("AdvanceAdjust", 0);
                reportDocument.SetParameterValue("OthersPayment", 0);
                reportDocument.SetParameterValue("Payment", aDmsSalesInvoice.DiscountAmount);

                reportDocument.SetParameterValue("InvoiceAmount", aDmsSalesInvoice.InvoiceAmount);
                reportDocument.SetParameterValue("AmountInWord", _library.TakaWords(aDmsSalesInvoice.InvoiceAmount));
                reportDocument.SetParameterValue("Remarks", aDmsSalesInvoice.Remarks ?? " ");
                reportDocument.SetParameterValue("FooterPrintCaption", "Printed By : " + loggedUserName);
                reportDocument.SetParameterValue("EmployeeName", loggedUserFullName);
                reportDocument.SetParameterValue("Barcode", barcodes);
                reportDocument.SetParameterValue("InvoiceTitle", "Invoice (DMS)");

                //Dummy Data Start
                reportDocument.SetParameterValue("IsVisible", true);
                reportDocument.SetParameterValue("IspercentDiscount?", false);
                reportDocument.SetParameterValue("IsVisibleCreditCard", true);
                reportDocument.SetParameterValue("PBalance", false);
                reportDocument.SetParameterValue("RBalance", false);
                reportDocument.SetParameterValue("CBalance", true);
                reportDocument.SetParameterValue("IsTML", true);
                reportDocument.SetParameterValue("IsCLP", false);

                reportDocument.SetParameterValue("DiscountDetails", discountDetails);


                //Dummy Data End

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            }
            catch (Exception ex)
            {
                AppLogger.PrintException(ex, "User Name: " + loggedUserName);
            }

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

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public byte[] imageToByteArray(Image imageIn)
        {

            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private void QrCodeGen(Consumer aConsumer, ProductDetails aProductDetails, DmsSalesInvoice aDmsSalesInvoice, WarrantyParam aWarrantyParam, string sBarcode)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE
            };
            try
            {
                qrCodeEncoder.QRCodeScale = 4;
                qrCodeEncoder.QRCodeVersion = 15;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                String data = aConsumer.ConsumerCode +
                "\n" + aDmsSalesInvoice.InvoiceNo +
                "\n" + aDmsSalesInvoice.InvoiceDate.ToShortDateString() +
                "\n" + aConsumer.ConsumerName +
                "\n" + aConsumer.Address +
                "\n" + aConsumer.ContactNo +
                "\n" + aConsumer.Email +
                "\n" + aProductDetails.ProductCode +
                "\n" + aProductDetails.ProductName +
                "\n" + sBarcode +
                "\n" + aWarrantyParam.ServiceWarranty +
                "\n" + aWarrantyParam.PartsWarranty +
                "\n" + aWarrantyParam.SpecialComponentWarranty +
                "\n" + 4 +
                "\n" + 5;
                _iImage = qrCodeEncoder.Encode(data);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }


    }


}