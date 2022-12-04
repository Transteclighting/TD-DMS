using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table ("t_EMITenure")]
    public class InstallmentNo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Tenure { get; set; }
    }
}