using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    [Table(" v_TDAttendanceDetail")]
    public class TDAttendanceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 SL { get; set; }
        public string Serial { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string ShowroomCode { get; set; }
        public string TerritoryName { get; set; }
        public string AreaName { get; set; }
        public string PunchDate { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string WorkingTime { get; set; }
        public string InAddress { get; set; }
        public string OutAddress { get; set; }
        public double? Distancemeters { get; set; }
        public string WorkingTimeInsideShowroom { get; set; }
        public string WorkingTimeOutsideShowroom { get; set; }
        public string InLatitude { get; set; }
        public string InLongitude { get; set; }
        public string OutLatitude { get; set; }
        public string OutLongitude { get; set; }
        public decimal? OutletLatitude { get; set; }
        public decimal? OutletLongitude { get; set; }
        public string AttendanceStatus { get; set; }
        public int FloorLeadQty { get; set; }
        public double FloorLeadAmount { get; set; }
        public int CollectedLeadQty { get; set; }
        public double CollectedLeadAmount { get; set; }
        public double WEBLeadAmount { get; set; }
        public double Conversion { get; set; }
    }
}