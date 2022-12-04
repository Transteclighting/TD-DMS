using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DealerManagementSystem.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
            //this.Z = z;
            //this.Y1 = y1;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public double Y = 0;


        //[DataMember(Name = "y1")]
        //public double Y1 = 0;
    }

    public class CalculatedData
    {

        public double TDTarget { get; set; }
        public double TDOrder { get; set; }
        public double LDTarget { get; set; }
        public double LDOrder { get; set; }
        public double LD_minus1_Order { get; set; }
        public double LDDelivery { get; set; }
        public double Bounce { get; set; }
        public double BouncePer { get; set; }

    }
}

