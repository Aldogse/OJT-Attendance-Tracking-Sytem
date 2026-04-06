using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class HrProfileVM
    {
        [Required(ErrorMessage = "FirstName is Required!")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "LastName is Required!")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;

        [Required(ErrorMessage = "Address is Required!")]
        [StringLength(50)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Contact Number is required")]
        public string ContactNumber { get; set; }
        public string UserId { get; set; } = null!;
    }
}
