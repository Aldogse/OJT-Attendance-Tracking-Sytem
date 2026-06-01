using System.Runtime.Versioning;
using System.Security.Claims;
using System.Threading.Tasks;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;


namespace Attendace_Tracking_Sytem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentRepository _studentRepository;

        public StudentController(IRegistrationRepository registrationRepository
            ,DatabaseContext databaseContext
            ,ILogger<StudentController>logger,
            IStudentRepository studentRepository)
        {
            _registrationRepository = registrationRepository;
            _databaseContext = databaseContext;
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public IActionResult SuccessRegistrationPage()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StudentProfileForm(string UserId)
        {
            var studentProfile = new StudentProfileVM();
            studentProfile.UserId = UserId;
            return View(studentProfile);
        }

        [HttpPost]
        public async Task<IActionResult> StudentProfileForm(StudentProfileVM StudentProfile)
        {

                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input! Try again");
                    return View(StudentProfile);
                }

                bool exist = await _databaseContext.StudentsProfile.AnyAsync(i => i.StudentId == StudentProfile.StudentId);
                
                if (exist)
                {
                    ModelState.AddModelError("","Student ID already exist!");
                    return View(StudentProfile);
                }
                var newStudent = await _registrationRepository.StudentProfileSetUp(StudentProfile);

                var profile = await _databaseContext.Users.Where(i => i.Id == newStudent.UserId).FirstOrDefaultAsync();

                if (profile != null)
                {
                    profile.ProfileCompleted = true;
                }

                if(profile == null)
                {
                    ModelState.AddModelError("","Profile is null!");
                    return View(StudentProfile);
                }

                await _databaseContext.SaveChangesAsync();
                
                return RedirectToAction("Success","Student");            

        }


        [HttpGet]
        public async Task<IActionResult> HrPendingStudentDetails(int ProfileId)
        {

                var student = await _studentRepository.PendingStudentWorkProfile(ProfileId);

                var studentVM = new StudentPendingWorkProfileVM()
                {
                    Department = student.Department,
                    EndDate = student.EndDate,
                    FullName = $"{student.FirstName} {student.MiddleName} {student.LastName}",
                    HoursRendered = student.HoursRendered,
                    RequiredHours = student.RequiredHours,
                    ShiftEnd = student.ShiftEnd,
                    ShiftStart = student.ShiftStart,
                    StartDate = student.StartDate,
                    Status = student.Status,
                    ProfileId = ProfileId
                };

                return View(studentVM);

        }

        //TIME IN AND TIME OUT LOGIC
        [HttpPost]
        public async Task<IActionResult> TimeIn()
        {

                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //GET USER ID THAT MATCHES THE CURRENT STUDENT    
             var student = await _databaseContext.StudentsProfile.FirstOrDefaultAsync(i => i.UserId == user);

                if(student == null)
                {
                TempData["TimeInError"] = "Error during Time in.";
                return RedirectToAction(nameof(StudentDashboard));
                }

                await _studentRepository.ClockIn(student.ProfileId,student.ShiftStart);
                return RedirectToAction("StudentDashboard", "Student");

        }

        [HttpPost]
        public async Task<IActionResult> TimeOut()
        {

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = await _databaseContext.StudentsProfile.AsNoTracking().Where(i => i.UserId == user)
                .FirstOrDefaultAsync();

            if(student == null)
            {
                ModelState.AddModelError("","User id is missing");
                return RedirectToAction(nameof(StudentDashboard));
            }

            bool isClockOut = await _studentRepository.ClockOut(student.ProfileId);

            if (!isClockOut)
            {
                ModelState.AddModelError("","No Clock in recorded, Please clock in.");
                return RedirectToAction(nameof(StudentDashboard));
            }

            return RedirectToAction("StudentDashboard", "Student");
        }

        //STUDENT DASHBOARD
        [HttpGet]
        public async Task<IActionResult> StudentDashboard(int page = 1)
        {

                var UserData = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(UserData == null)
                {
                    return RedirectToAction("Account", "LoginPage");
                }

                var Student = await _studentRepository.GetStudentDashboardData(UserData,page);
                Student.StudentLogs = await _studentRepository.PaginatedStudentLog(UserData,Student.CurrentPage);
                
                return View(Student);
        }

        [HttpGet]
        public async Task<IActionResult> MissedTimeoutDetails(int ProfileId)
        {

                var TimeoutDetails = await _databaseContext.MissedTimeouts.Include(i => i.Profile).Where(i => i.ProfileId == ProfileId)
                    .Select(i => new StudentMissedLogDetailsVM
                    {
                        Department = i.Profile.Department,
                        Fullname = $"{i.Profile.FirstName} {i.Profile.MiddleName} {i.Profile.LastName}",
                        Logdate = i.LogDate,
                        Explanation = i.Explanation,  
                        ProfileId = ProfileId, 
                        Timeout = i.Timeout,
                    }).FirstOrDefaultAsync();

                return View(TimeoutDetails);
        }

        [HttpPost]
        public async Task<IActionResult> MissedTimeoutDetails(StudentMissedLogDetailsVM exp)
        {
                var MissedLog = await _studentRepository.GetMissedLog(exp.ProfileId);

                MissedLog.Explanation = exp.Explanation;
                MissedLog.Timeout = exp.Timeout;
                await _databaseContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Explanation submitted successfully!";
                return RedirectToAction("StudentDashboard","Student"); ;
        }

        [HttpPost]
        public async Task<IActionResult> UploadNBI(IFormFile NBIFile, int ProfileId)
        {

                //CHECK IF FILE NULL OR EMPTY
                if (NBIFile == null || NBIFile.Length < 0)
                {
                    TempData["Error"] = "No file Attached!";
                    return RedirectToAction("StudentRequirementUploadPage","Student");
                }

                var allowedExt = new[] { ".jpg", ".jpeg", ".png" };
                var fileExt = Path.GetExtension(NBIFile.FileName).ToLower();

                if(!allowedExt.Contains(fileExt))
                {
                    TempData["Error"] = "Invalid file type! Only .jpg, .jpeg, and .png are allowed.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //VALID Multipurpose Internet Mail Extensions
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };

                //CHECK IF YUNG CONTENT TYPE NG FILE AY VALID
                if(!allowedMimeTypes.Contains(NBIFile.ContentType))
                {
                    TempData["Error"]= "Invalid file type! Only JPEG and PNG images are allowed.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //CHECK KUNG YUNG FILE SIZE AY 2 MB LANG 
                if(NBIFile.Length > 2 * 1024 * 1024)
                {
                   TempData["Error"] = "File size exceed the maximum Limit.";
                   return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                bool uploadResult =  await _studentRepository.UploadNBI(ProfileId, NBIFile, fileExt);

                if(!uploadResult)
                {
                    ModelState.AddModelError("","Failed to upload NBI Clearance. Please try again.");
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                TempData["Success"] = "NBI Clearance uploaded successfully!";
                return RedirectToAction("StudentRequirementUploadPage", "Student");
            }
        

        [HttpPost]
        public async Task<IActionResult> UploadMOA(IFormFile MOAFile, int ProfileId)
        {

                //check if file ay may laman
                if(MOAFile == null || MOAFile.Length == 0)
                {
                    TempData["Error"] = "No file selected!";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //Valid format ng file 
                var allowedExt = new[] { ".jpg", ".jpeg", ".png" };

                //CHECK KUNG TAMA YUNG FILE Format
                var fileExt = Path.GetExtension(MOAFile.FileName).ToLower();

                if (!allowedExt.Contains(fileExt))
                {
                    TempData["Error"] = "Invalid file type! Only .jpg, .jpeg, and .png are allowed.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //VALID MINE TYPE 
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedMimeTypes.Contains(MOAFile.ContentType))
                {
                    TempData["Error"] = "Invalid Content type! Only JPEG and PNG images are allowed.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //CHECK IF THE SIZE IS VALID
                if (MOAFile.Length > 2 * 1024 * 1024)
                {
                    TempData["Error"] = "File size exceed the maximum limit of 2 MB.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                bool upload = await _studentRepository.UploadMOA(ProfileId, MOAFile, fileExt);

                if (!upload)
                {
                    TempData["Error"] = "Error uploading file, Try again!";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                TempData["Success"] = "Succcessfully upload Memorandum of Agreement.";
                return RedirectToAction("StudentRequirementUploadPage", "Student");
            
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile StudentIdFile, int ProfileId)
        {
                if(StudentIdFile == null || StudentIdFile.Length == 0)
                {
                    TempData["Error"] = "Please attach a file";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                var fileExt = Path.GetExtension(StudentIdFile.FileName).ToLower();
                //valid file extensions 
                var validExt = new[] { ".jpg",".jpeg",".img"};

                if (!validExt.Contains(fileExt))
                {
                    TempData["Error"] = "Please attach a valid file extension!";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //CHECK IF THE CONTENT IS VALID
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedMimeTypes.Contains(StudentIdFile.ContentType))
                {
                    TempData["Error"] = "Invalid content!";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                //check if the size of the file is valid
                if(StudentIdFile.Length > 2 * 1024 * 1024)
                {
                    TempData["Error"] = "Allowed size is only 2MB.";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                bool uploadImage = await _studentRepository.UploadProfilePicture(StudentIdFile, ProfileId,fileExt);

                if (!uploadImage)
                {
                    TempData["Error"] = "Issue uploading the File, Try again!";
                    return RedirectToAction("StudentRequirementUploadPage", "Student");
                }

                TempData["Success"] = "File successfully Uploaded!";
                return RedirectToAction("StudentRequirementUploadPage", "Student");


        }

        [HttpGet]
        public async Task<IActionResult> StudentRequirementUploadPage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["Error"] = "User Id is missing!";
                return View();
            }

            int ProfileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == userId).Select(i => i.ProfileId)
                .FirstOrDefaultAsync();

            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);

            var viewModel = new StudentRequirementsPageVM()
            {
                ProfileId = ProfileId,
                MemorandumOfAgreementImagePath = student?.MemorandumOfAgreementImagePath,
                NBIImagePath = student?.NbiImagePath,
                StudentIdImagePath = student?.StudentIdImagePath,
            };

            return View(viewModel);          
        }

        [HttpGet]
        public async Task<IActionResult> StudentRequirements()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["Error"] = "User Id is missing!";
                return View();
            }

            int ProfileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == userId).Select(i => i.ProfileId)
               .FirstOrDefaultAsync();

            var student = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == ProfileId);


            var viewModel = new StudentRequirementStatusUpdatePageVM()
            {
                StudentIdImagePath = student?.StudentIdImagePath,
                MemorandumOfAgreementImagePath = student?.MemorandumOfAgreementImagePath,
                NBIImagePath = student?.NbiImagePath,
                Message = student?.Message,
                Verified = student?.Verified
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceLogs()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //pass the profile id on the view so that when the post happen it can be filtered
            var profileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == userId).Select(i => i.ProfileId)
                .FirstOrDefaultAsync();
             
            ViewBag.ProfileId = profileId;

            return View(new List<StudentLogVM>());
        }

        [HttpPost]
        public async Task<IActionResult> AttendanceLogs(DateOnly? startDate,DateOnly? endDate)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var profileId = await _databaseContext.StudentsProfile.Where(i => i.UserId == userId)
                    .Select(i => i.ProfileId).FirstOrDefaultAsync();

                var logs = new List<StudentLogVM>();

                if(startDate != null && endDate != null)
                {
                    logs = await _databaseContext.StudentLogs.Where
                        (i => i.ProfileId == profileId &&
                        i.LogDate >= startDate && i.LogDate <= endDate
                        ).Select(i => new StudentLogVM
                        {
                            Logdate = i.LogDate,
                            Timein = i.TimeIn.ToString(@"hh\:mm"),
                            Timeout = i.TimeOut != null 
                            ? i.TimeOut.Value.ToString(@"hh\:mm") 
                            : "--",
                            TotalHours = i.TotalHours,
                        }).ToListAsync();
                }

                return View (logs);
        }

        [HttpGet]
        public IActionResult StudentDailyAttendanceReport()
        {
            return View();
        }
    }
}
