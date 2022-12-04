using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }
        public string BankName { get; set; }
        public double Amount { get; set; }
        public string CardType { get; set; }
        public string CardNo { get; set; }
        public string IsEMI { get; set; }
        public int InstallmentNo { get; set; }
        public string POSMachine { get; set; }

    }
}