using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class SecOrderViewModel
    {
        public int TranID { get; set; }
        [DisplayName("RouteID")]
        public int BeatID { get; set; }
        public string RouteName { get; set; }
        public string RetailName { get; set; }
        public int RetailID { get; set; }
        public string TranDate { get; set; }
        public decimal NetAmount { get; set; }
    }
}