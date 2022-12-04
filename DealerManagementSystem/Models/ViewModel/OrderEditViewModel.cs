using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models.ViewModel
{
    public class OrderEditViewModel
    {
        public int OrderId { get; set; }
        public List<ProductForOrder> OrderProducts { get; set; }
        public bool IsOrderToTaggedShowroom { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
    }
}