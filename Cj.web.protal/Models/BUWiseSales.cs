using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class BUWiseSales
    {
        public string Description { get; set; }
        public double Today { get; set; }
        public double LastDay { get; set; }
        public double CMTarget { get; set; }
        public double MTDTarget { get; set; }
        public double ThisMonth { get; set; }
        public double LMTarget { get; set; }
        public double LastMonth { get; set; }
        public double ThisYear { get; set; }
        public double LYYTD { get; set; }
        public double LastYear { get; set; }
        public double YBLY { get; set; }
        public int Sort { get; set; }
        public double CMPer { get; set; }
        public double MTDPer { get; set; }
        public double LMPer { get; set; }
        public double YTDGth { get; set; }

}
}