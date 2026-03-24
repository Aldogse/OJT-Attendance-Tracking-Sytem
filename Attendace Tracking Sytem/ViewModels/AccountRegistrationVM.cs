using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels
{
    public class AccountRegistrationVM
    {
        [Required(ErrorMessage = "Email is Required!")]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50,MinimumLength = 8)]
        public string Password { get; set; }


        [StringLength(50)]
        [Compare("Password",ErrorMessage ="Confirm password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
