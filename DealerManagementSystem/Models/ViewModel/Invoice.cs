using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models.ViewModel
{
    public class Invoice
    {
        List<Invoice> inv = new List<Invoice>();
        public string InvoiceNo { get; set; }
        public int IsDistSysDBTran { get; set; }
    }
}