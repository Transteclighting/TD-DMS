using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_DMSPromoAppliedToInvoice")]
    public class DmsPromoAppliedToInvoice
    {
        [Key]
        public int Id { get; set; }
        public string PromoType { get; set; }
        public int  PromoId { get; set; }
        public int  ProductId { get; set; }
        public int InvoiceId { get; set; }
        public double Amount { get; set; }
    }
}