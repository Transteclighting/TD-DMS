using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSProductStock")]
    public class DmsProductStock
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int DistributorId { get; set; }
        public int CurrentStock { get; set; }

    }
}