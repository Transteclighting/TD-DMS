using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace DealerManagementSystem.Models
{
    [Table("t_WarrantyParam")]
    public class WarrantyParam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WarrantyParamId { get; set; }
        public int ServiceWarranty { get; set; }
        public int PartsWarranty { get; set; }
        public int SpecialComponentWarranty { get; set; }
        public int IsCurrent { get; set; }
    }
}