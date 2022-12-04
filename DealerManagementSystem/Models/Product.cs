using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("t_Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string ProductModel { get; set; }
        public int ProductGroupId { get; set; }
        [Display(Name="Brand Name")]
        public int BrandId { get; set; }

        //public string SmallUnitOfMeasure { get; set; }
        //public string LargeUnitOfMeasure { get; set; }
        ////public double UomConversionFactor { get; set; }
        //public DateTime EntryDate { get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public int ProductType { get; set; }
        ////public int IsActive { get; set; }
        //public int HsCodeId { get; set; }
        //public string MidUnitOfMeasure { get; set; }
        //public int LsRatio { get; set; }
        //public int MsRatio { get; set; }
        //public int SupplyType { get; set; }
        //public int VatApplicable { get; set; }
        //public string ProductSbUs { get; set; }
        ////public int UploadStatus { get; set; }
        //public DateTime UploadDate { get; set; }
        //public DateTime DownloadDate { get; set; }
        ////public int RowStatus { get; set; }
        ////public int InventoryCategory { get; set; }
        ////public int ItemCategory { get; set; }
        //public int IsBarcodeItem { get; set; }
        
        //public ICollection<DmsProductStockSerial> DmsProductSerials { get; set; }

        [ForeignKey("ProductGroupId")]
        public virtual ProductGroup ProductGroup { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand{ get; set; }

    }
}





