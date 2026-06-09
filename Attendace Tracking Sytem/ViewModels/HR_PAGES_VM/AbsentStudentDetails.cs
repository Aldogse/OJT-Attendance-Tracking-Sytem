using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class AbsentStudentDetails
    {
        public string FirstName { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public Departments Department { get; set; }

    }
}
