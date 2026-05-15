using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class HrStudentRequirementsVM
    {
        public int profileId { get; set; }
        public string fullName { get; set; } = string.Empty;
        public string nbiImagePath { get; set; } = string.Empty;
        public string memorandumOfAgreementImagePath { get; set; } = string.Empty;
        public string studentIdImagePath { get; set; } = string.Empty;
        public string? message { get; set; } = string.Empty;
        public bool? Verified { get; set; }
    }
}
