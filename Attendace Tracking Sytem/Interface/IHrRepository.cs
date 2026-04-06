using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IHrRepository
    {
        Task<HrDashBoardVM> HrDashboardInformation(string UserId);
        Task ApproveStudentWorkProfile(int Id);
        Task DenyStudentWorkProfile(int Id);
    }
}
