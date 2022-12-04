using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_DmsInvoiceDiscount")]
    public class DmsInvoiceDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DiscountId { get; set; }
        public int InvoiceId { get; set; }
        public Int16 DiscountTypeId { get; set; }
        public double Amount { get; set; }
    }
}