using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSProductStockSerial")]
    public class DmsProductStockSerial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public int TranId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int ProductId { get; set; }
        [Display(Name = "Barcode SL#")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 2)]
        public string ProductSerial { get; set; }
        public int Status { get; set; }

        public string OriginalSerial { get; set; }
        //[ForeignKey("ProductId")]
        //public virtual Product Product { get; set; }

    }
}