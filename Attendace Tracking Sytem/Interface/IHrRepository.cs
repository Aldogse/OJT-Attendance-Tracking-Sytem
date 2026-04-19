using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IHrRepository
    {
        Task<HrDashBoardVM> HrDashboardInformation(string UserId);
        Task ApproveStudentWorkProfile(int Id);
        Task DenyStudentWorkProfile(int Id);
        Task<List<StudentLogs>> MissedTimeOuts(DateOnly date);
        Task<StudentMissedLogDetailsVM> MissTimeoutDetails(int ProfileId);
        Task ApproveMissedLog(int ProfileId, DateOnly Logdate);
    }
}
