using System.Data;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.DTO;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.Services;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Attendace_Tracking_Sytem.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<StudentRepository> _logger;
        private readonly CloudinaryService _cloudinaryService;
        private readonly Cloudinary _cloudinary;

        public StudentRepository(DatabaseContext databaseContext,ILogger<StudentRepository>logger,CloudinaryService cloudinaryService,Cloudinary cloudinary)
        {
            _databaseContext = databaseContext;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _cloudinary = cloudinary;
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

        public async Task<bool> UploadNBI(int ProfileId, IFormFile file)
        {
            var student = await  _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            if (student == null)
            {
                return false;
            }

            if (student.NbiImagePath != null)
            {
                //DELETE EXISTING FILE IN CLOUDINARY
                var deleteParams = new DeletionParams(student.NbiImagePublicId)
                {
                    ResourceType = ResourceType.Image,
                    Invalidate = true
                };

                await _cloudinary.DestroyAsync(deleteParams);
                var newNBIPath = await _cloudinaryService.UploadImage(file);
                //ADD NEW IMAGE 
                student.NbiImagePath = newNBIPath.ImageUrlPath;
                student.NbiImagePublicId = newNBIPath.ImageId;
                await _databaseContext.SaveChangesAsync();
                return true;
            }

            var NBIPath = await _cloudinaryService.UploadImage(file);
            //ADD NEW IMAGE 
            student.NbiImagePath = NBIPath.ImageUrlPath;
            student.NbiImagePublicId = NBIPath.ImageId;
            await _databaseContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UploadMOA(int ProfileId, IFormFile file)
        {
            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            if (student == null)
                return false;

            //IF THERE IS AN EXISTING PICTURE DELETE IT AND UPLOAD A NEW ONE
            if(student.MemorandumOfAgreementImagePath != null)
            {
                var deleteParam = new DeletionParams(student.MemorandumOfAgreementImagePublicId)
                {
                    ResourceType = ResourceType.Image,
                    Invalidate = true
                };

                await _cloudinary.DestroyAsync(deleteParam);
                var newMoaPath = await _cloudinaryService.UploadImage(file);
                student.MemorandumOfAgreementImagePath = newMoaPath.ImageUrlPath;
                student.MemorandumOfAgreementImagePublicId = newMoaPath.ImageId;
                await _databaseContext.SaveChangesAsync();
                return true;
            }

            var MoaPath = await _cloudinaryService.UploadImage(file);
            student.MemorandumOfAgreementImagePath = MoaPath.ImageUrlPath;
            student.MemorandumOfAgreementImagePublicId = MoaPath.ImageId;
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadProfilePicture(IFormFile file, int ProfileId)
        {
            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            if (student == null)
                return false;

            if(student.StudentIdImagePath != null)
            {
                var delParams = new DeletionParams(student.StudentIdImagePublicId)
                {
                    Invalidate = true,
                    ResourceType = ResourceType.Image,
                };

                await _cloudinary.DestroyAsync(delParams);

                //new image
                var newProfilePic = await _cloudinaryService.UploadImage(file);
                student.StudentIdImagePath = newProfilePic.ImageUrlPath;
                student.StudentIdImagePublicId = newProfilePic.ImageId;
                await _databaseContext.SaveChangesAsync();
                return true;
            }

            var ProfilePic = await _cloudinaryService.UploadImage(file);
            student.StudentIdImagePath = ProfilePic.ImageUrlPath;
            student.StudentIdImagePublicId = ProfilePic.ImageId;
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

        public async Task<StudentApplication> StudentApplicationProcess(StudentApplicationDTO studentProfileDTO, string? extension)
        {
            StudentApplication application = new StudentApplication
            {
                FullName = studentProfileDTO.FullName,
                Course = studentProfileDTO.Course,
                Description = studentProfileDTO.Description,
                Year = studentProfileDTO.Year,
                School = studentProfileDTO.School,
                ApplicationDate = DateTime.UtcNow,
                Status = Enums.ApplicationStatus.Pending
            };

            if(studentProfileDTO.Resume != null)
            {
                var imageName = Guid.NewGuid().ToString() + extension;

                //FOLDER
                var folder = Path.Combine
                (
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "students",
                "resumes"
                );

                //CREATE DIRECTOR IF NOT EXIST
                Directory.CreateDirectory(folder);

                var newResumePath = Path.Combine(folder, imageName);
                using var stream = new FileStream(newResumePath,FileMode.Create);
                await studentProfileDTO.Resume.CopyToAsync(stream);

                application.ResumePath = $"/students/resumes/{newResumePath}";
                await _databaseContext.StudentApplications.AddAsync(application);
                await _databaseContext.SaveChangesAsync();
                return application;
            }

            await _databaseContext.StudentApplications.AddAsync(application);
            await _databaseContext.SaveChangesAsync();
            return application;

        }

    }
}
