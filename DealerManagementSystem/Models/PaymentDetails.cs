using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSSalesInvoicePaymentMode")]
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      
        public int DataID { get; set; }
        public int InvoiceID { get; set; }
        public Int16 ProductID { get; set; }
        public Int16 PaymentModeID { get; set; }
        public int BankID { get; set; }
        public Int16 CardType { get; set; }
        public Int16 POSMachineID { get; set; }
        public int IsEMI { get; set; }
        public Int16 NoOfInstallment { get; set; }
        public Int16 InstrumentNo { get; set; }
        public DateTime InsTrumentDate { get; set; }
        public Int16 CardCategory { get; set; }
        public Int16 ApprovalNo { get; set; }


        
    }
}