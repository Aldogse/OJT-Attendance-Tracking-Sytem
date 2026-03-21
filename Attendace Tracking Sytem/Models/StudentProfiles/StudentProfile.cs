using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required]
        [StringLength(50)]
        public string StudentId { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string School { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string MiddleName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
