using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [StringLength(11)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public Departments Department { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public decimal? HoursRendered { get; set; }
        public decimal RequiredHours { get; set; } = 300;
        public TimeOnly ShiftStart { get; set; } = new TimeOnly(8, 0);
        public TimeOnly ShiftEnd { get; set; } = new TimeOnly(17, 0);
        public Status Status { get; set; } = Status.Pending;

        //CREDENTIALS ID
        [ForeignKey("User")]
        
        public string UserId { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public LogInCredentials User { get; set; }
        public List<MissedTimeouts>?MissedLogs { get; set; }
    }
}
