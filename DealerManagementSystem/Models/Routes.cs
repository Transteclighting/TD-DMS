using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSRoute")]
    public class Routes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RouteID { get; set; }
        public string RouteName { get; set; }

        List<Routes> Route = new List<Routes>();
    }
}