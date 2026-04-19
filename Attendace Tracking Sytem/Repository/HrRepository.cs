using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Repository
{
    public class HrRepository : IHrRepository
    {
        private readonly DatabaseContext _databaseContext;

        public HrRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task ApproveMissedLog(int ProfileId,DateOnly Logdate) 
        {
            var log = await _databaseContext.MissedTimeouts.FirstOrDefaultAsync(i => i.ProfileId == ProfileId && i.LogDate == Logdate);
            var studentLogs = await _databaseContext.StudentLogs.Include(i => i.StudentProfile).FirstOrDefaultAsync(i => i.ProfileId == ProfileId && i.LogDate == Logdate);

            studentLogs.TimeOut = log.Timeout;
            studentLogs.TotalHours = (decimal)(studentLogs.TimeOut - studentLogs.TimeIn)?.TotalHours;
            studentLogs.Status = Enums.AttendanceStatus.Complete;
            studentLogs.StudentProfile.HoursRendered = studentLogs.TotalHours;
            log.isApproved = true;

            await _databaseContext.SaveChangesAsync();

        }

        public async Task ApproveStudentWorkProfile(int Id)
        {
            var student = new StudentProfile { ProfileId = Id };


            _databaseContext.StudentsProfile.Attach(student);

            student.Status = Enums.Status.Active;

            _databaseContext.StudentsProfile.Entry(student)
                .Property(i => i.Status).IsModified = true;

            await _databaseContext.SaveChangesAsync();
        }

        public Task DenyStudentWorkProfile(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<HrDashBoardVM> HrDashboardInformation(string UserId)
        {
            DateTime date = DateTime.Now;
            var UserData = await _databaseContext.HRProfile.Where(i => i.UserId == UserId).FirstOrDefaultAsync();

            var pendingStatus = await _databaseContext.StudentsProfile.Where(i => i.Status == Enums.Status.Pending)
                .ToListAsync();

            var numberOfActiveStudents = await _databaseContext.StudentsProfile.Where(i => i.Status == Enums.Status.Active)
                .CountAsync();

            var NumOfFinishingStudents = await _databaseContext.StudentsProfile.Where(i => i.EndDate.Month == date.Month).CountAsync();
            
            var MissedLogs = await _databaseContext.MissedTimeouts.Where(i => i.isApproved == false && i.LogDate.Month == date.Month 
            && i.LogDate.Year == date.Year)
                .OrderBy(i => i.LogDate)
                .ToListAsync();

            return new HrDashBoardVM
            {
                NumberOfActiveStudents = numberOfActiveStudents,
                FinishingStudents = NumOfFinishingStudents,
                PendingStudents = pendingStatus,
                FullName = $"{UserData.FirstName} {UserData.MiddleName} {UserData.LastName}",
                MissedTimeouts = MissedLogs
            };
        }

        //BACKGROUND SERVICE 
        public async Task<List<StudentLogs>> MissedTimeOuts(DateOnly date)
        {
            var logs = await _databaseContext.StudentLogs.Where(i => i.LogDate == date && i.TimeOut == null)
                .ToListAsync();

            return logs;
        }

        public async Task<StudentMissedLogDetailsVM> MissTimeoutDetails(int ProfileId)
        {
            var log = await _databaseContext.MissedTimeouts
                .Include(i => i.Profile).FirstOrDefaultAsync(i => i.ProfileId == ProfileId);

            var logVm = new StudentMissedLogDetailsVM
            {
                Department = log.Profile.Department,
                Fullname = $"{log.Profile.FirstName} {log.Profile.MiddleName} {log.Profile.LastName}",
                Explanation = log.Explanation,
                Logdate = log.LogDate,
                Timeout = log.Timeout,
                ProfileId = ProfileId
            };

            return logVm;
                
        }
    }
}
