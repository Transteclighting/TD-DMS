using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{

    [Table("t_DMSOrderItem")]
    public class DMSOrderItem
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TranID { get; set; }
        [Key, Column(Order = 1)]
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public double UnitPrice { get; set; }

        public int? SaleQty { get; set; }
        public int? FreeQty { get; set; }
        public int? RepIssueQty { get; set; }
        public int? RepRecQty { get; set; }


    }
}