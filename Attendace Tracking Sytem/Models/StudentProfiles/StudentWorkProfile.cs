using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Attendace_Tracking_Sytem.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentWorkProfile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StudentProfile")]
        public int? StudentProfileId { get; set; } = null;

        [JsonIgnore]
        [ValidateNever]
        public StudentProfile? StudentProfile { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public decimal? HoursRendered { get; set; }
        public decimal RequiredHours { get; set; } = 300;
        public TimeOnly ShiftStart { get; set; } = new TimeOnly(8,0);
        public TimeOnly ShiftEnd { get; set; } = new TimeOnly(17,0);
        public Status Status { get; set; } = Status.Pending;  

    }
}
