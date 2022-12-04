using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models.ViewModel
{
    public class SerialandStock
    {
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public string ProductSerial { get; set; }
    }
}