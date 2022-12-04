using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSSalesInvoiceWarrantyCardNo")]
    public class WarrantyCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WarrantyCardId { get; set; }
        public int OutletId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string WarrantyCardNo { get; set; }
        public string ProductSerialNo { get; set; }
        public int WarrantyParameterId { get; set; }
    }
}





