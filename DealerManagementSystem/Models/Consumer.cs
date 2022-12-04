using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_DMSConsumer")]
    public class Consumer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ConsumerId { get; set; }
        public int CustomerId { get; set; }
        public string ConsumerCode { get; set; }
        [Display(Name = "Customer Name")]
        [Required(ErrorMessage = "Please Enter Consumer Name")]
        public string ConsumerName { get; set; }


        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please Enter Customer Address")]
        public string Address { get; set; }
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [Display(Name = "Contact No")]
        [Required(ErrorMessage = "Please Enter Contact No")]
        public string ContactNo { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please Enter Email Address")]
        // [EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public DateTime DateofBirth { get; set; }
        public int? ReffID { get; set; }



        //public ICollection<DmsSalesInvoice> DmsSalesInvoces { get; set; }

    }
}






