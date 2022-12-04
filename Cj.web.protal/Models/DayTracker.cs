using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class DayTracker
    {
        public int TrackId { get; set; }
        public string CheckTime { get; set; }
        public int EmployeeId { get; set; }
        public string Type { get; set; }
        public int InstrumentId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Remarks { get; set; }
        public string Address { get; set; }
        public int? MergeAttendanceData { get; set; }
    }
}