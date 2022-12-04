using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class CombinedModel
    {

        public SecOrder SecOrder { get; set; }
        public Routes Routes { get; set; }
        public Outlets Outlets { get; set; }
    }
}