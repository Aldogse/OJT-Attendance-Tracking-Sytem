using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentProfileVM
    {
        public string UserId { get; set; }

        [Required(ErrorMessage ="Student ID is required!")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "School is required!")]
        public string School { get; set; }

        [Required(ErrorMessage = "Firstname is required!")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Middlename is required!")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Lastname is required!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Age is required!")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phonenumber is required!")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
