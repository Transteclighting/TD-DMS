using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class SkuWiseSales
    {
        public string AreaName { get; set; }
        public string TerritoryName { get; set; }
        public string ShowroomCode { get; set; }
        public int TYear { get; set; }
        public int TMonth { get; set; }
        public int WeekNo { get; set; }
        public string PGName { get; set; }
        public string MAGName { get; set; }
        public string ASGNAme { get; set; }
        public string AGName { get; set; }
        public string Branddesc { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int TargetQty { get; set; }
        public int SalesQty { get; set; }
        public Int64 CurrentStock { get; set; }
    }
}