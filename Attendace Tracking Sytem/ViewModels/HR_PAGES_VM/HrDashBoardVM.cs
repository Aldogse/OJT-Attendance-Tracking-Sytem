using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;

namespace Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM
{
    public class HrDashBoardVM
    {
        public string FullName { get; set; } = string.Empty;
        public int NumberOfActiveStudents { get; set; }
        public List<StudentProfile>? PendingStudents { get; set; }

        //HR DASHBOARD ANALYTICS
        public double? monthAbsentism { get; set; }
        public double? monthLateRate { get; set; }
        public List<DailyAttendanceReport>? attendanceTrend {  get; set; }
    }
}
