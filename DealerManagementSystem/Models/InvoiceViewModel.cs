
using System.Collections.Generic;

namespace DealerManagementSystem.Models
{
    public class InvoiceViewModel
    {

        public int InvoiceViewModelId { get; set; }
        public DmsSalesInvoice DmsSalesInvoice { get; set; }
        public Consumer Consumer { get; set; }
        public List<DmsDiscountType> DmsDiscountTypes { get; set; }
        public List<DmsInvoiceDiscount> DmsInvoiceDiscount { get; set; }
        public List<DmsSalesInvoiceDetail> DmsSalesInvoiceDetail { get; set; }
        public DmsSecondarySalesOrder DmsSecondarySalesOrder { get; set; }
        public List<DmsSecondarySalesOrderDetail> DmsSecondarySalesOrderDetail { get; set; }


    }
}