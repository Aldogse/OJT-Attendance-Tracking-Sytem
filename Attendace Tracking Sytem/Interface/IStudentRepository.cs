using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IStudentRepository
    {
        Task<StudentProfile> PendingStudentWorkProfile(int Id);
        Task<StudentLogs> ClockIn(int? Id);
        Task<StudentLogs> ClockOut(int? Id);
        Task<StudentDashboardVM> GetStudentDashboardData(string UserId);
        Task<MissedTimeouts> GetMissedLog(int ProfileId);
    }
}
