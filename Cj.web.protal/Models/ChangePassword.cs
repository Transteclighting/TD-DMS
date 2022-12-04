using System.ComponentModel.DataAnnotations;


namespace Cj.web.Models
{
    public class ChangePassword
    {
        [Display(Name = "UserName")]
        public string UserName{ get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Old Password")]
        [Display(Name = "Old Password")]

        public string OldPassword { get; set; }
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter New Password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [CompareAttribute("NewPassword", ErrorMessage = "Confirm Password Mismatch With New Password.")]
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Salt { get; set; }
    }
}