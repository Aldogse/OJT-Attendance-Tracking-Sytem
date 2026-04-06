using Attendace_Tracking_Sytem.Models.StudentProfiles;

namespace Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM
{
    public class HrDashBoardVM
    {
        public string FullName { get; set; }
        public int NumberOfActiveStudents { get; set; }
        public List<StudentWorkProfile> PendingStudents { get; set; }
        public int FinishingStudents { get; set; }

    }
}
