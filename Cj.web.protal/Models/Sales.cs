using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class Sales
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MAGName { get; set; }
        public double? TargetQty { get; set; }
        public double? TargetValue { get; set; }
        public int SalesQty { get; set; }
        public double QtyPer { get; set; }
        public double? NetSale { get; set; }
        public double ValPer { get; set; }
    }
}