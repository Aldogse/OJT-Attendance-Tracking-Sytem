using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IHrRepository
    {
        Task<HrDashBoardVM> HrDashboardInformation(string UserId,DateOnly? startDate,DateOnly? endDate);
        Task ApproveStudentWorkProfile(int Id);
        Task DenyStudentWorkProfile(int Id);
        Task<List<StudentLogs>> MissedTimeOuts(DateOnly date);
        Task<StudentMissedLogDetailsVM> MissTimeoutDetails(int ProfileId);
        Task ApproveMissedLog(int ProfileId, DateOnly Logdate,string UserId);
        Task<List<StudentsMissedLogsVM>> StudentMissedLogsFiltered(MissedLogStatus? missedLogStatus,DateOnly? date);
        Task<List<StudentVM>> GetStudents(int page = 1,Departments? department = null);
        Task<List<HrStudentRequirementsVM>> GetStudentRequirementsVMs(int page = 1);
        Task<StudentsDetailsVM> GetStudentProfile(int ProfileId);
        Task<StudentLogSummaryVM> GetStudentLogSummary(int ProfileId,int page = 1,DateOnly? StartDate = null,DateOnly? EndDate = null);
        //DATE RANGE
        Task<DailyAttendanceReportVM> DailyAttendanceReport(DateOnly? start, DateOnly? end);

        //NOTIFY HR BY EMAIL AND POST IT AS A LIST IN THE DASHBOARD
        Task<List<AbsentStudentDetails>> GetStudentWith3absencesOrMore();

        //APPLICATIONS
        Task<List<StudentApplicationsVM>> GetStudentApplicants(int page ,int size);
        Task<StudentApplicationsVM> GetStudentApplication(int ApplicationId);
    }
}
