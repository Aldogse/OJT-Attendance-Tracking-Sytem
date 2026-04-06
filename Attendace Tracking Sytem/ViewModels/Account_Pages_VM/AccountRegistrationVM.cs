using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels.Account_Pages_VM
{
    public class AccountRegistrationVM
    {
        [Required(ErrorMessage = "Email is Required!")]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50, MinimumLength = 8)]
        [Required(ErrorMessage ="Password is Required!")]
        public string Password { get; set; } = null!;


        [StringLength(50)]
        [Compare("Password",ErrorMessage ="Confirm password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
