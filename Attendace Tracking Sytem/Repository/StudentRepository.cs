using System.Data;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(DatabaseContext databaseContext,ILogger<StudentRepository>logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public async Task DroppedStudentWorkProfile(int Id)
        {
        }

        public async Task<StudentProfile> PendingStudentWorkProfile(int ProfileId)
        {
            StudentProfile? studentWorkProfile = await _databaseContext.StudentsProfile
                .Where(i => i.ProfileId == ProfileId)
                .FirstOrDefaultAsync();

            return studentWorkProfile;
        }

        //CLOCK IN AND CLOCK OUT FUNCTIONS
        public async Task<StudentLogs> ClockIn(int? ProfileId)
        {
            try
            {
                var logs = new StudentLogs()
                {
                    ProfileId = ProfileId,
                    LogDate = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)),
                    TimeIn = DateTime.Now,
                    TimeOut = null
                };

                await _databaseContext.StudentLogs.AddAsync(logs);
                await _databaseContext.SaveChangesAsync();
                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new StudentLogs();
            }
        }

        public async Task<StudentLogs> ClockOut(int? ProfileId)
        {
            DateOnly date = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            var studentData = await _databaseContext.StudentLogs.Where(i => i.ProfileId == ProfileId && i.LogDate == date)
                .FirstOrDefaultAsync();

            studentData.TimeOut = DateTime.Now;
            studentData.Status = Enums.AttendanceStatus.Complete;

            TimeSpan totalHours = studentData.TimeOut.Value - studentData.TimeIn;
            studentData.TotalHours = Math.Round((decimal)totalHours.TotalHours, 2);

            return studentData;
        }

        //DASHBOARD DATA QUERIES
        public async Task<StudentDashboardVM> GetStudentDashboardData(string UserId)
        {
            StudentDashboardVM? student = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId)
                .Select(i => new StudentDashboardVM
                {
                    Department = i.Department,
                    EndDate = i.EndDate.ToShortDateString(),
                    StartDate = i.StartDate.ToShortDateString(),
                    FullName = $"{i.FirstName} {i.MiddleName} {i.LastName}",
                    HoursRendered = i.HoursRendered,
                }).FirstOrDefaultAsync();

                int ProfileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId)
                    .Select(i => i.ProfileId).FirstOrDefaultAsync();

            List<StudentLogs> logs = await _databaseContext.StudentLogs.Where(i => i.ProfileId == ProfileId).ToListAsync();
          
            student.StudentLogs = logs;

            return student;

        }

        public async Task<List<StudentLogs>> MissedTimeOuts(DateOnly date)
        {
            List<StudentLogs> missedTimeout = await _databaseContext.StudentLogs.Where(i => i.LogDate == date && i.TimeOut == null)
                .ToListAsync();

            return missedTimeout;
        }
    }
}
