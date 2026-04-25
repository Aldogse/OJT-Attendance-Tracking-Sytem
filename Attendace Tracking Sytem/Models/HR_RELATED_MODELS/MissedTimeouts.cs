using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS
{
    public class MissedTimeouts
    {
        [Key]
        public int Id { get; set; }
        public int LogId { get; set; }

        public int? ProfileId { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public StudentProfile? Profile { get; set; }

        public DateOnly LogDate { get; set; }
        public string? Explanation { get; set; }
        public TimeSpan? Timeout {  get; set; }
        public MissedLogStatus status { get; set; } = MissedLogStatus.Pending;
        public int? ApproverProfileId { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public HRProfile? HrProfile { get; set; }
    }
}


