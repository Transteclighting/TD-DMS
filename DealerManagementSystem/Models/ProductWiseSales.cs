using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class ProductWiseSales
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int AsgId { get; set; }
        public string AsgName { get; set; }
        public int SalesQuantity { get; set; }
    }
}