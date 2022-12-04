using System.ComponentModel.DataAnnotations;

namespace DealerManagementSystem.Enums
{
    public enum CreditCardType
    {
        [Display(Name = "Visa")]
        VISA = 1,
        [Display(Name = "Master")]
        MASTER = 2,
        [Display(Name = "Amex")]
        AMEX = 3,
        [Display(Name = "NEXUS")]
        NEXUS = 4,
        [Display(Name = "JCB")]
        JCB = 5,
    }
}
