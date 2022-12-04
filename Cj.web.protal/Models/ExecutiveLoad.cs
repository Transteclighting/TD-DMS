using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class ExecutiveLoad
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int weekno { get; set; }
        public int cmonth { get; set; }
        public int cyear { get; set; }
        public string theDate { get; set; }

        public int? Load { get; set; }
        public decimal? Amount { get; set; }

    }
}