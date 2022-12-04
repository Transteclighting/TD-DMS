using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cj.web.Models
{
    [Table("V_DistributionPlanDetails")]
    public class DistributionPlanDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeCode { get; set; }
        //public string CustType { get; set; }
        //public string AreaName { get; set; }
        //public string TerritoryName { get; set; }
        //public string ThanaName { get; set; }
        //public string DistrictName { get; set; }
        //public DateTime? PlanDate { get; set; }
        //public int? PYear { get; set; }
        //public int? PMonth { get; set; }
        public int NoOfPlan { get; set; }
        public int NoOfVisit { get; set; }
        public int NoOfInvoice { get; set; }
        public double? NetSales { get; set; }
        public int SalesQty { get; set; }
    }
}