
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{

    public class DmsProductStock_A
    {

        public int ProductId { get; set; }
        public int DistributorId { get; set; }
        public int CurrentStock { get; set; }

    }
}