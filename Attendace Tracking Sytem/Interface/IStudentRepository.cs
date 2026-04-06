using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IStudentRepository
    {
        Task<StudentWorkProfile> PendingStudentWorkProfile(int Id);
        Task<StudentLogs> ClockIn(int? Id);
        Task<StudentLogs> ClockOut(int? Id);
        Task<StudentDashboardVM> GetStudentDashboardData(string UserId);
    }
}
