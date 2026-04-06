using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels.Account_Pages_VM
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Email is Required!")]
        public string EmailAddress { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required!")]
        public string Password { get; set; } = null!;
    }
}
