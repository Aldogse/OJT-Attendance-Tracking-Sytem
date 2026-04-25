using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;

namespace Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM
{
    public class HrDashBoardVM
    {
        public string FullName { get; set; }
        public int NumberOfActiveStudents { get; set; }
        public List<StudentProfile>? PendingStudents { get; set; }
        public int FinishingStudents { get; set; }

    }
}
