using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DealerManagementSystem.Models
{
    [Table("t_DMSCLusterOutlet")]
    public class Outlets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RetailID { get; set; }
        public string RetailName { get; set; }
        public string RetailAddress { get; set; }
        public string Mobile01 { get; set; }
        public string Email { get; set; }



    }
}