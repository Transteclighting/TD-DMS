using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class Attendance
    {
        public string DSRCode { get; set; }
        public string DSRName { get; set; }
        public string TerritoryName { get; set; }
        public string Designation { get; set; }
        public string TDAtten { get; set; }
        public string LDAtten { get; set; }
        public string TD_ISLATE { get; set; }
        public string LD_ISLATE { get; set; }
        public string TD_FV { get; set; }
        public string LD_FV { get; set; }
        public string TD_LV { get; set; }
        public string LD_LV { get; set; }
        public string TDWH { get; set; }
        public string LDWH { get; set; }

    }
}