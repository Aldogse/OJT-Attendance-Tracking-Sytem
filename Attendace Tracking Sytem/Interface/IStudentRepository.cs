using Attendace_Tracking_Sytem.DTO;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IStudentRepository
    {
        Task<StudentProfile> PendingStudentWorkProfile(int Id);
        Task<StudentLogs> ClockIn(int? Id,TimeOnly shiftStart);
        Task<bool> ClockOut(int? Id);
        Task<StudentDashboardVM> GetStudentDashboardData(string UserId,int page);
        Task<MissedTimeouts> GetMissedLog(int ProfileId);
        Task<List<StudentLogVM>> PaginatedStudentLog(string UserId,int page);
        Task<bool> UploadNBI(int ProfileId,IFormFile file);
        Task<bool> UploadMOA(int ProfileId,IFormFile file);
        Task<bool> UploadProfilePicture(IFormFile file,int ProfileId);
        Task<List<int>> GetProfileIds();
        Task<DailyAttendanceReport> GetDailyAttendanceReport(DateOnly logDate);
        Task<StudentProfile> GetStudentByUserId(string userId);
        Task<StudentApplication> StudentApplicationProcess(StudentApplicationDTO studentProfileDTO,string? extension);
        
    }
}
