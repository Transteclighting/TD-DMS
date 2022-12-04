using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CrystalDecisions.CrystalReports.Engine;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSProductStockTranItem")]
    public class DmsProductStockTranItem
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TranId { get; set; }
        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        public int Qty { get; set; }
        public double CostPrice { get; set; }
        public double SalesPrice { get; set; }
    }
}