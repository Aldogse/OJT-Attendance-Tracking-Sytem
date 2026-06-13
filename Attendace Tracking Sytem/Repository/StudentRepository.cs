using System.Data;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

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
        public async Task<StudentLogs> ClockIn(int? ProfileId,TimeOnly shiftStart)
        {
            TimeSpan currentTime = DateTime.UtcNow.TimeOfDay;
            TimeOnly clockInTime = new TimeOnly(currentTime.Hours,currentTime.Minutes,currentTime.Seconds);

            var logs = new StudentLogs()
            {
                ProfileId = ProfileId,
                LogDate = DateOnly.FromDateTime(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day)),
                TimeIn = currentTime,
                TimeOut = null,
                isAbsent = false,
            };

            if(clockInTime > shiftStart)
            {
                logs.isLate = true;
            }   

            await _databaseContext.StudentLogs.AddAsync(logs);
            await _databaseContext.SaveChangesAsync();
            return logs;
        }

        public async Task<bool> ClockOut(int? ProfileId)
        {
            DateOnly date = DateOnly.FromDateTime(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day));

            //i checheck kung nakapag clock in na si user
            var studentData = await _databaseContext.StudentLogs.FirstOrDefaultAsync(i => i.ProfileId == ProfileId && i.LogDate == date);
                
            if(studentData == null)
            {
                return false;
            }

            studentData.TimeOut = DateTime.UtcNow.TimeOfDay;
            studentData.Status = Enums.AttendanceStatus.Complete;

            studentData.TotalHours = studentData.TimeOut - studentData.TimeIn;

            await _databaseContext.SaveChangesAsync();

            return true;
        }

        //DASHBOARD DATA QUERIES
        public async Task<StudentDashboardVM> GetStudentDashboardData(string UserId,int page)
        {
            int size = 1;
            DateTime date = DateTime.UtcNow;
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
           DateTime date = DateTime.UtcNow;
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
            var student = await  _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);
            var NbiFilename = Guid.NewGuid().ToString() + ext;

            var folderPath = Path.Combine
                (
                Directory.GetCurrentDirectory(),
                "wwwroot", "students", "images"
                );

            //combine the path
            var filePath = Path.Combine(folderPath, NbiFilename);

            if (student == null)
            {
                var newNbiImage = new StudentRequirements()
                {
                    NbiImagePath = $"/students/images/{NbiFilename}",
                    StudentProfileId = ProfileId,                                       
                };
                //Create folder path if it doesn't exist
                Directory.CreateDirectory(folderPath);

                //SAVE THE FILE ON THE SERVER 
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _databaseContext.StudentRequirements.AddAsync(newNbiImage);
                await _databaseContext.SaveChangesAsync();
                return true;
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

            //Create folder path if it doesn't exist
            Directory.CreateDirectory(folderPath);

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
            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            var newMOAFilename = Guid.NewGuid().ToString() + ext;

            var folderPath = Path.Combine
           (
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "students",
                "images"
           );

            //CREATE FOLDER PATH IF IT DOESN'T EXIST
            Directory.CreateDirectory(folderPath);

            var newMOAFilePath = Path.Combine(folderPath, newMOAFilename);       

            if (student == null)
            {
                var newMOAImage = new StudentRequirements()
                {
                    StudentProfileId = ProfileId,
                    MemorandumOfAgreementImagePath = $"/students/images/{newMOAFilename}"
                };

                using (var stream = new FileStream(newMOAFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _databaseContext.StudentRequirements.AddAsync(newMOAImage);
                await _databaseContext.SaveChangesAsync();
                return true;
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
            //save file on the server 
            using (var stream = new FileStream(newMOAFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            student.MemorandumOfAgreementImagePath = $"/students/images/{newMOAFilename}";

            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadProfilePicture(IFormFile file, int ProfileId, string? ext)
        {
            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            if (student == null)
            {
                return false;
            }

            //icheck kung may picture yung student pag meron burahin at palitan nung bagong picture
            if (!string.IsNullOrEmpty(student.StudentIdImagePath))
            {
                var oldPath = Path.Combine
                    (
                      Directory.GetCurrentDirectory(),
                      "wwwroot",
                      student.StudentIdImagePath.TrimStart('/')
                    );

                if(File.Exists(oldPath))
                    File.Delete(oldPath);

            }

            var newImageName = Guid.NewGuid().ToString() + ext;

            var folder = Path.Combine
                (
                 Directory.GetCurrentDirectory(),
                 "wwwroot",
                 "students",
                 "images"
                );

            Directory.CreateDirectory(folder);
            var newImageFolderPath = Path.Combine(folder, newImageName);


            using var stream = new FileStream(newImageFolderPath, FileMode.Create);
            await file.CopyToAsync(stream);

            student.StudentIdImagePath = $"/students/images/{newImageName}";
            await _databaseContext.SaveChangesAsync();
            return true;
            
        }

        //
        public async Task<List<int>> GetProfileIds()
        {
            var ids = await _databaseContext.StudentsProfile.Select(i => i.ProfileId).ToListAsync();
            return ids;
        }


        //BACK GROUND SERVICE FUNCTIONS
        public async Task<DailyAttendanceReport> GetDailyAttendanceReport(DateOnly logDate)
        {
            var attendance = await _databaseContext.StudentLogs.Where(i => i.LogDate == logDate)
               .ToListAsync();

            int absents = await _databaseContext.StudentLogs.Where(i => i.isAbsent == true).CountAsync();
            int present = await _databaseContext.StudentLogs.Where(i => i.isAbsent == false).CountAsync();
            int lates = await _databaseContext.StudentLogs.Where(i => i.isLate == true).CountAsync();

            var attendanceReport = new DailyAttendanceReport
            {
                attendanceDate = logDate,
                numberOfAbsents = absents,
                numberOfLates = lates,
                numberOfPresent = present
            };

            return attendanceReport;

        }

        public async Task<StudentProfile> GetStudentByUserId(string userId)
        {
            return await _databaseContext.StudentsProfile.FirstOrDefaultAsync(i => i.UserId == userId);
        }
    }
}
