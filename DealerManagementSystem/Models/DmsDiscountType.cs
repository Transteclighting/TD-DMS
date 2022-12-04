using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_DMSDiscountType")]
    public class DmsDiscountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int16 DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}