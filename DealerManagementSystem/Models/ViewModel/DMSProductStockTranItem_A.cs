using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;

namespace DealerManagementSystem.Models
{

    public class DmsProductStockTranItem_A
    {


        public int ProductId { get; set; }
        public int Qty { get; set; }
        public double CostPrice { get; set; }
        public double SalesPrice { get; set; }
    }
}