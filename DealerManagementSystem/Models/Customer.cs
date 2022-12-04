using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_Customer")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        public int? ParentCustomerId { get; set; }
        //public ICollection<DmsSalesInvoice> DmsSalesInvoces { get; set; }

    }
    
}

