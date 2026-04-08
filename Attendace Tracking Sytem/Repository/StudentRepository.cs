using System.Data;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
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

        public async Task<StudentWorkProfile> PendingStudentWorkProfile(int Id)
        {
            var studentWorkProfile = await _databaseContext.StudentsWorkProfile.Include(i => i.StudentProfile)
                .Where(i => i.Id == Id)
                . FirstOrDefaultAsync();

            return studentWorkProfile;
        } 
        
        //CLOCK IN AND CLOCK OUT FUNCTIONS
        public async Task<StudentLogs> ClockIn(int? StudentWorkProfileId)
        {
            try
            {
                var logs = new StudentLogs()
                {
                    StudentWorkProfileId = StudentWorkProfileId,
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

        public async Task<StudentLogs> ClockOut(int? Id)
        {
            DateOnly date = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day));
            var studentData = await _databaseContext.StudentLogs.Where(i => i.StudentWorkProfileId == Id && i.LogDate == date).FirstOrDefaultAsync();

            studentData.TimeOut = DateTime.Now;
            studentData.Status = Enums.AttendanceStatus.Complete;

            TimeSpan totalHours = studentData.TimeOut.Value - studentData.TimeIn;
            studentData.TotalHours = Math.Round((decimal)totalHours.TotalHours,2);

            return studentData;
        }

        //DASHBOARD DATA QUERIES
        public async Task<StudentDashboardVM>  GetStudentDashboardData(string UserId)
        {
            var Student = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId).Select(i => i.ProfileId)
                .FirstOrDefaultAsync();

            var StudentData = await _databaseContext.StudentsWorkProfile.Include(i => i.StudentProfile)
                .Where(i => i.StudentProfileId == Student).FirstOrDefaultAsync();

            var logs = await _databaseContext.StudentLogs.Where(i => i.StudentWorkProfileId == StudentData.Id).ToListAsync();

            var studentWorkHours = _databaseContext.StudentLogs.Where(i => i.StudentWorkProfileId == StudentData.Id)
            .Sum(i => i.TotalHours);

            return new StudentDashboardVM
            {
                Department = StudentData.Department,
                FullName = $"{StudentData.StudentProfile.FirstName} {StudentData.StudentProfile.MiddleName} {StudentData.StudentProfile.LastName}",
                EndDate = StudentData.EndDate.ToShortDateString(),
                StartDate = StudentData.StartDate.ToShortDateString(),
                HoursRendered = studentWorkHours,
                StudentLogs = logs,
            };
           
        }
    }
}
