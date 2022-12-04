﻿
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{

    public class DmsProductStockTran_A
    {


        public string TranNo { get; set; }
        public DateTime TranDate { get; set; }
        public int TranType { get; set; }
        public int TranSide { get; set; }
        public int FromCustomerId { get; set; }
        public int ToCustomerId { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int RefInvoiceId { get; set; }
        public double InvoiceAmount { get; set; }
        public double DiscountAmount { get; set; }
        public int IsDistSysDBTran { get; set; }
    }
}
