using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_Showroom")]
    public class Showroom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int ShowroomId { get; set; }
        public string ShowroomCode { get; set; }
        public string ShowroomName { get; set; }
        public int CustomerId { get; set; }
        public int WarehouseId { get; set; }
       
    }

}

