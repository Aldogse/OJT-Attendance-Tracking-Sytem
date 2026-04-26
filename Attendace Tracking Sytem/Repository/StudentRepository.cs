using System.Data;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Mvc;
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
                    TimeIn = DateTime.Now.TimeOfDay,
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

            studentData.TimeOut = DateTime.Now.TimeOfDay;
            studentData.Status = Enums.AttendanceStatus.Complete;

            studentData.TotalHours = (decimal)(studentData.TimeOut - studentData.TimeIn)?.TotalHours;

            return studentData;
        }

        //DASHBOARD DATA QUERIES
        public async Task<StudentDashboardVM> GetStudentDashboardData(string UserId,int page)
        {
            int size = 1;
            DateTime date = DateTime.Now;
            StudentDashboardVM? student = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId)
                .Select(i => new StudentDashboardVM
                {
                    Department = i.Department,
                    EndDate = i.EndDate.ToShortDateString(),
                    StartDate = i.StartDate.ToShortDateString(),
                    FullName = $"{i.FirstName} {i.MiddleName} {i.LastName}",
                    HoursRendered = i.HoursRendered, 
                    CurrentPage = page
                }).FirstOrDefaultAsync();


                int ProfileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId)
                    .Select(i => i.ProfileId).FirstOrDefaultAsync();

            List<MissedTimeouts> missed = await _databaseContext.MissedTimeouts.Where(i => i.ProfileId == ProfileId
            && i.status == Enums.MissedLogStatus.Pending).ToListAsync();

            student.MissedTimeouts = missed;

            return student;

        }

        public async Task<MissedTimeouts> GetMissedLog(int ProfileId)
        {
            MissedTimeouts? student = await _databaseContext.MissedTimeouts.FirstOrDefaultAsync(i => i.ProfileId == ProfileId);

            return student;
        }

        public async Task<List<StudentLogVM>> PaginatedStudentLog(string UserId, int page)
        {
           DateTime date = DateTime.Now;
            int size = 5;

            int ProfileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == UserId).Select(i => i.ProfileId).FirstOrDefaultAsync();

            List<StudentLogVM>? logs = await _databaseContext.StudentLogs.Where(i => i.ProfileId == ProfileId && i.LogDate.Month == date.Month
            && i.LogDate.Year == date.Year)
                .Select(i => new StudentLogVM
                {
                      Logdate = i.LogDate,
                      Timein = i.TimeIn.ToString(@"hh\:mm"),
                      Timeout  = i.TimeOut != null 
                      ? i.TimeOut.Value.ToString(@"hh\:mm")
                      : "--",
                      TotalHours = i.TotalHours,
                })
                .OrderByDescending(i => i.Logdate)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return logs;
        }

        public async Task<bool> UploadNBI(int ProfileId, IFormFile file,string? ext)
        {
            var student = await  _databaseContext.StudentRequirements.FindAsync(ProfileId);

            if (student == null)
            {
                return false;
            }

            //UPLOAD LOGIC

            //DELETE OLD IMAGE IF IT EXISTS 
            if (!string.IsNullOrEmpty(student.NbiImagePath))
            {
                var oldPath = Path.Combine
                (
                    Directory.GetCurrentDirectory(), 
                    "wwwroot", 
                    student.NbiImagePath.TrimStart('/')                  
                );

                //DELETE EXISTING FILE
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            var NbiFilename = Guid.NewGuid().ToString() + ext;

            var folderPath = Path.Combine
                (
                   Directory.GetCurrentDirectory(),
                   "wwwroot","students","images"
                );

            //Create folder path if it doesn't exist
            Directory.CreateDirectory(folderPath);

            //combine the path
            var filePath = Path.Combine(folderPath, NbiFilename);

            //SAVE THE FILE ON THE SERVER 
            using (var stream = new FileStream(filePath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //SAVE THE PATH TO THE DATABASE
            student.NbiImagePath = $"/students/images/{NbiFilename}";

            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadMOA(int ProfileId, IFormFile file, string? ext)
        {
            var student = await _databaseContext.StudentRequirements.FindAsync(ProfileId);
            if (student == null)
            {
                return false;
            }

            //UPLOAD LOGIC
            //DELETE OLD FILE PATH
            if (!string.IsNullOrEmpty(student.MemorandumOfAgreementImagePath))
            {
                var oldPath = Path.Combine(
                 Directory.GetCurrentDirectory(),
                  "wwwroot",
                  student.MemorandumOfAgreementImagePath.TrimStart('/')
                );

                //DELETE THE OLD PATH
                if(System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            var newMOAFilename = Guid.NewGuid().ToString() + ext;

            var newMOAFilePath = Path.Combine
                (
                     Directory.GetCurrentDirectory(),
                     "wwwroot",
                     "students",
                     "images",
                     newMOAFilename
                );

            //CREATE FOLDER PATH IF IT DOESN'T EXIST
            Directory.CreateDirectory(newMOAFilePath);

            //save file on the server 
            using (var stream = new FileStream(newMOAFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            student.MemorandumOfAgreementImagePath = $"images/students/{newMOAFilePath}";

            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public Task<bool> UploadProfilePicture(IFormFile file, int ProfileId, string? ext)
        {
            throw new NotImplementedException();
        }
    }
}
