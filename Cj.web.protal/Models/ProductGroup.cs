using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cj.web.Models
{
    [Table("t_ProductGroup")]
    public class ProductGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PdtGroupId { get; set; }
        [Display(Name = "MAG")]
        public string PdtGroupName { get; set; }

        public int GroupSort { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}