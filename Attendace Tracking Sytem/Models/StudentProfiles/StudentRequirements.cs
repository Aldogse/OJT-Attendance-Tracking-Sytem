using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentRequirements
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StudentProfile")]
        public int StudentProfileId { get; set; }

        [ValidateNever]
        public StudentProfile StudentProfile { get; set; } = null!;

        public string? NbiImagePath { get; set; }

        public string? NbiImagePublicId { get; set; }

        public string? MemorandumOfAgreementImagePath { get; set; }

        public string? MemorandumOfAgreementImagePublicId { get; set; }

        public string? StudentIdImagePath { get; set; }

        public string? StudentIdImagePublicId { get; set; }

        public bool Verified { get; set; } = false;

        public string? Message { get; set; }
    }
}
