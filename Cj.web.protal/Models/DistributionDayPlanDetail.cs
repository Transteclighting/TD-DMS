using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class DistributionDayPlanDetail
    {
        public string PunchDate { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string SubDepartmentName { get; set; }
        public string PlanDate { get; set; }
        public string PlanNo { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string Purpose { get; set; }
        public string PlanAddress { get; set; }
        public string IsOldCustomer { get; set; }
        public string CompetitionName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ThanaName { get; set; }
        public string DistrictName { get; set; }
        public decimal? CustLongitude { get; set; }
        public decimal? CustLatitude { get; set; }
        public decimal? CheckInLatitude { get; set; }
        public decimal? CheckInLongitude { get; set; }
        public decimal? CheckOutLatitude { get; set; }
        public decimal? CheckOutLongitude { get; set; }
        public string CheckInAddress { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutAddress { get; set; }
        public string CheckOutTime { get; set; }
        public string PlanStatus { get; set; }
        public double? Distancemeters { get; set; }
    }
}