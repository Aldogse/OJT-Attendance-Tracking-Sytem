using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        public StudentProfile Profile { get; set; }

        public DateOnly LogDate { get; set; }
        public bool Missed { get; set; } = true;
    }
}


