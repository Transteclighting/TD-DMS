using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class DistributionAttendance
    {
        public string PunchDate { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyCode { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string WorkingTime { get; set; }
        public string InAddress { get; set; }
        public string OutAddress { get; set; }
        public string OutLatitude { get; set; }
        public string InLatitude { get; set; }
        public string OutLongitude { get; set; }
        public string InLongitude { get; set; }
        public string TotalWorkingMinutes { get; set; }
        public string CustLatitude { get; set; }
        public string CustLongitude { get; set; }
        public double? Distancemeters { get; set; }
        public int NoOfPlan { get; set; }
        public int NoOfVisit { get; set; }
        public int NoOfInvoice { get; set; }
        public double? NetSales { get; set; }
        public int SalesQty { get; set; }
    }
}