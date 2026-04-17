using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentDashboardVM
    {
        public string FullName { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public decimal? HoursRendered { get; set; } = null!;
        public string Department { get; set; } = null!;
        public List<StudentLogs>? StudentLogs { get; set; } = null!;
        public List<MissedTimeouts>?MissedTimeouts { get; set; } = null!;
    }
}
