using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSSecondarySalesOrder")]
    public class DmsSecondarySalesOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int WarehouseId { get; set; }
        public int SalesType { get; set; }
        public int CustomerId { get; set; }
        public int ParentCustomerId { get; set; }
        public DateTime Edd { get; set; }
        public double OrderAmount { get; set; }
        public int Status { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string RefInvoiceNo { get; set; }
        public DateTime? RefInvoiceDate { get; set; }
        public int? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Remarks { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthorizeDate { get; set; }
        public string AuthorizeRemarks { get; set; }
        

    }
}