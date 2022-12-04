using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class ChartView
    {

        public class MTD_Pri
        {
            public string RegionName { get; set; }
            public double PriSales { get; set; }
            public double SecSales { get; set; }

        }

        public class MTD_Sec
        {
            public string RegionName { get; set; }
            public double SecSales { get; set; }
            //  public double Sec { get; set; }
        }

        public class MTD_PriTGT
        {
            public string RegionName { get; set; }
            public double PriTGT { get; set; }
            //  public double Sec { get; set; }
        }

        public class MTD_SecTGT
        {
            public string RegionName { get; set; }
            public double SecTGT { get; set; }
            //  public double Sec { get; set; }
        }

        public class MonthWiseSales
        {
            public string Months { get; set; }
            public double Sales { get; set; }
            //  public double Sec { get; set; }
        }

        public class DisWiseSales_Sec
        {
            public string District { get; set; }
            public double Sec { get; set; }
            //  public double Sec { get; set; }
        }

        public class National_TGT
        {
            public string Nationals { get; set; }
            public double Target { get; set; }

        }

        public class National_Sales
        {
            public string Nationals { get; set; }
            public double Sales { get; set; }

        }

        public class National_TGT_YTD
        {
            public string Nationals { get; set; }
            public double Target { get; set; }

        }

        public class National_Sales_YTD
        {
            public string Nationals { get; set; }
            public double Sales { get; set; }

        }

        public class StrikeRates
        {
            public string RegionName { get; set; }
            public double StrikeRate { get; set; }

        }


        public class Bounce
        {
            public string Name { get; set; }
            public double Value { get; set; }

        }
    }
}