using System.ComponentModel.DataAnnotations;
using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentApplication
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Fullname is Required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "School is Required")]
        public string School { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course is Required")]
        public string Course { get; set; } = string.Empty;

        public string? ResumePath { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is Required")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime ApplicationDate { get; set; }
    }
}
