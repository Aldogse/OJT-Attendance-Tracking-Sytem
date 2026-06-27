using System.ComponentModel.DataAnnotations;
using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentApplicationsVM
    {
        public string FullName { get; set; } = string.Empty;

        public string School { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string? ResumePath { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Description { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime ApplicationDate { get; set; }
    }
}
