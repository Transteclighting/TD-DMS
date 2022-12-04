using System.Collections.Generic;

namespace DealerManagementSystem.Models.ViewModel
{
    public class OrderCreateViewModel
    {
        public List<ProductForOrder> OrderProducts { get; set; }
        public bool IsOrderToTaggedShowroom { get; set; }
        public string Remarks { get; set; }
        public int? CustomerId { get; set; }
    }
}