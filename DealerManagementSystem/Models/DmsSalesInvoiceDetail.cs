using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_DMSSalesInvoiceDetail")]
    public class DmsSalesInvoiceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string BarcodeSerial { get; set; }
        public double UnitPrice { get; set; }
        public double CostPrice { get; set; }
        public bool IsFreeProduct { get; set; }
        public int FreeQty { get; set; }  
    }
}


