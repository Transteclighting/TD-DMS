
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{

    public class DmsProductStockSerial_A
    {

        public int TranId { get; set; }
        public int ProductId { get; set; }
        public string ProductSerial { get; set; }
        public int Status { get; set; }
        public string OriginalSerial { get; set; }
        //[ForeignKey("ProductId")]
        //public virtual Product Product { get; set; }

    }
}