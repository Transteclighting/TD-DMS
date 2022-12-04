using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSUser")]
    public class DmsUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string UserFullName { get; set; }
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Enter User Name First")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter Password First")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Salt { get; set; }
        public int CreateUserId { get; set; }
        public string  Role { get; set; }
        public int? IsActive { get; set; }

        //public string UserSbUs { get; set; }

    }

    public class User
    {
        public string Password { get; set; }
        public string Salt { get; set; }
        public int? EmployeeID { get; set; }
    }

}