namespace DealerManagementSystem.Models.ViewModel
{
    public class ProductViewModel
    {
        public bool IsChecked { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string  ProductSerial { get; set; }
        public string  ProductName { get; set; }
        public int AsgId { get; set; }
        public string  AsgName { get; set; }
        public string  MagName { get; set; }
        public string  BrandDesc { get; set; }
        public double Rsp { get; set; }
        public double CostPrice { get; set; }
        public double PromoDiscount { get; set; }
        
    }
    public class OrderProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int OrderQty { get; set; }
    }

    public class OrderItemViewModel
    {

       // public int TranID { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        //public int SaleQty { get; set; }
        //public int FreeQty { get; set; }
        //public int RepIssueQty { get; set; }
        //public int RepRecQty { get; set; }

    }
}