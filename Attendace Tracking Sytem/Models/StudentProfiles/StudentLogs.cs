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

        public int? StudentWorkProfileId { get; set; }

        [ValidateNever]
        public StudentWorkProfile StudentWorkProfile { get; set; }

        [Required]
        public DateOnly LogDate { get; set; }

        public DateTime TimeIn { get; set; }

        public DateTime TimeOut { get; set; }

        public decimal? TotalHours { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Incomplete;
    }
}
