using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Attendace_Tracking_Sytem.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentLogs
    {
        [Key]
        public int LogId { get; set; }

        public int? ProfileId { get; set; }

        [ValidateNever]
        public StudentProfile StudentProfile { get; set; }

        [Required]
        public DateOnly LogDate { get; set; }

        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; } = null;

        public decimal? TotalHours { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Incomplete;
    }
}
