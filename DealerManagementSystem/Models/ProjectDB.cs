using System.Data.Entity;


namespace DealerManagementSystem.Models
{
    public class ProjectDb : DbContext
    {
        public ProjectDb() : base("TEL_DMS_ConnectionString")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
        public DbSet<DmsSalesInvoice> DmsSalesInvoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<DmsProductStockSerial> DmsProductStockSerials { get; set; }
        public DbSet<DmsUser> DmsUsers { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        //public DbSet<InvoiceViewModel> InvoiceViewModels { get; set; }
        public DbSet<DmsSalesInvoiceDetail> DmsSalesInvoiceDetail { get; set; }
        public DbSet<DmsProductStockTran> DmsProductStockTran { get; set; }
        public DbSet<DmsOutlet> DmsOutlet { get; set; }
        public DbSet<WarrantyCard> WarrantyCards { get; set; }
        public DbSet<WarrantyParam> WarrantyParams { get; set; }
        public DbSet<DmsProductStock> DmsProductStocks { get; set; }
        public DbSet<DmsProductStockTranItem> DmsProductStockTranItems { get; set; }
        public DbSet<DmsDiscountType> DmsDiscountTypes { get; set; }
        public DbSet<DmsInvoiceDiscount> DmsInvoiceDiscounts { get; set; }
        public DbSet<DmsPromoAppliedToInvoice> DmsPromoCpAppliedToInvoices { get; set; }
        public DbSet<DmsSecondarySalesOrder> DmsSecondarySalesOrder { get; set; }
        public DbSet<DmsSecondarySalesOrderDetail> DmsSecondarySalesOrderDetail { get; set; }
        public DbSet<Showroom> Showroom { get; set; }
        public DbSet<DataTransfer> DataTransfers { get; set; }
        public DbSet<Banks> Banks { get; set; }
        public DbSet<POSMachine> POSMachine { get; set; }
        public DbSet<InstallmentNo> InstallmentNo { get; set; }
        public DbSet<PaymentMode> PaymentMode { get; set; }
        public DbSet<PaymentDetails> Details { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Outlets> Outlets { get; set; }
        public DbSet<SecOrder> SecOrder { get; set; }
        public DbSet<DMSOrderItem> OrderItem { get; set; }
        //public DbSet<DataPoint> DataPoint { get; set; }


    }
}