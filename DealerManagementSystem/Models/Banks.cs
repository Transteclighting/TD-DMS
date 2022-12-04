using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{

    [Table("t_Bank")]
    public class Banks
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int BankID { get; set; }
        public string Name { get; set; }
    }
}