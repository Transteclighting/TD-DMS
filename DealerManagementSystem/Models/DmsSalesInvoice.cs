using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DealerManagementSystem.Models
{
    [Table("t_DMSSalesInvoice")]
    public class DmsSalesInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceId { get; set; }
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        public int InvoiceTypeId { get; set; }
        public int CustomerId { get; set; }
        [Display(Name = "Invoice Amount")]
        public double InvoiceAmount { get; set; }
        [Display(Name = "DiscountAmount")]

        [Required(ErrorMessage = "Please Enter Discount Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Please Enter Valid Discount Amount")]
        public double DiscountAmount { get; set; }
         [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int ConsumerId { get; set; }
        public Int16? Status { get; set; }

        public DateTime? ReverseApplyDate { get; set; }

        public int? ReverseReviseUserId { get; set; }
        public DateTime? ReverseReviseDate { get; set; }

        public int? RefInvoiceId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ConsumerId")]
        public virtual Consumer Consumer { get; set; }

    }
}


