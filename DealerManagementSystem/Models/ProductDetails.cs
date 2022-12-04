using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("v_ProductDetails")]
    public class ProductDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
        [Display(Name = "Product Code")]
        [NotMapped]
        public bool IsChecked { get; set; }
        [NotMapped]
        public string ProductSerial { get; set; }
        public string ProductCode { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public int AsgId { get; set; }
        public string AsgName { get; set; }
        public int MagId { get; set; }
        public string MagName { get; set; }
        //public string BrandName { get; set; }
        public int BrandId { get; set; }
        
        public string BrandDesc { get; set; }
        public double CostPrice { get; set; }
        //public double Price { get; set; }
        public double Rsp { get; set; }
    }
}