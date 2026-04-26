using System.Runtime.Versioning;
using System.Security.Claims;
using System.Threading.Tasks;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Identity.Client;

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
            try
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

                bool email = await _databaseContext.StudentsProfile.AnyAsync(i => i.Email == StudentProfile.Email);

                if(email)
                {
                    ModelState.AddModelError("", "Email already exist!");
                    return View(StudentProfile);
                }
                var newStudent = await _registrationRepository.StudentProfileSetUp(StudentProfile);

                var profile = await _databaseContext.Users.Where(i => i.Id == newStudent.UserId).FirstOrDefaultAsync();

                profile.ProfileCompleted = true;
                await _databaseContext.SaveChangesAsync();
                
                return RedirectToAction("Success","Student");            
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error:{ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> HrPendingStudentDetails(int ProfileId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        //TIME IN AND TIME OUT LOGIC
        [HttpPost]
        public async Task<IActionResult> TimeIn()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //GET USER ID THAT MATCHES THE CURRENT STUDENT    
                var student = await _databaseContext.StudentsProfile.AsNoTracking().Where(i => i.UserId == user)
                    .FirstOrDefaultAsync();

                if(student == null)
                {
                    return RedirectToAction("Error","Home");
                }

                await _studentRepository.ClockIn(student.ProfileId);
                return RedirectToAction("StudentDashboard", "Student");
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> TimeOut()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var student = await _databaseContext.StudentsProfile.AsNoTracking().Where(i => i.UserId == user)
                    .FirstOrDefaultAsync();


                await _studentRepository.ClockOut(student.ProfileId);

                await _databaseContext.SaveChangesAsync();
                return RedirectToAction("StudentDashboard", "Student");
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        //STUDENT DASHBOARD
        [HttpGet]
        public async Task<IActionResult> StudentDashboard(int page = 1)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MissedTimeoutDetails(int ProfileId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MissedTimeoutDetails(StudentMissedLogDetailsVM exp)
        {
            try
            {
                var MissedLog = await _studentRepository.GetMissedLog(exp.ProfileId);

                MissedLog.Explanation = exp.Explanation;
                MissedLog.Timeout = exp.Timeout;
                await _databaseContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Explanation submitted successfully!";
                return RedirectToAction("StudentDashboard","Student"); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadNBI(IFormFile file,int ProfileId)
        {
            try
            {
                //CHECK IF FILE NULL OR EMPTY
                if (file == null || file.Length < 0)
                {
                    TempData["ErrorMessage"] = "No file selected!";
                    return View();
                }

                var allowedExt = new[] { ".jpg", ".jpeg", ".png" };
                var fileExt = Path.GetExtension(file.FileName).ToLower();

                if(!allowedExt.Contains(fileExt))
                {
                    TempData["ErrorMessage"] = "Invalid file type! Only .jpg, .jpeg, and .png are allowed.";
                    return View();
                }

                //VALID Multipurpose Internet Mail Extensions
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };

                //CHECK IF YUNG CONTENT TYPE NG FILE AY VALID
                if(!allowedMimeTypes.Contains(file.ContentType))
                {
                    TempData["ErrorMessage"] = "Invalid file type! Only JPEG and PNG images are allowed.";
                    return View();
                }

                //CHECK KUNG YUNG FILE SIZE AY 2 MB LANG 
                if(file.Length > 2 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File size exceed the maximum Limit.";
                    return View();
                }

                bool uploadResult =  await _studentRepository.UploadNBI(ProfileId, file, fileExt);

                if(!uploadResult)
                {
                    TempData["UploadError"]  = "Failed to upload NBI Clearance. Please try again.";
                    return View();
                }

                TempData["SuccessMessage"] = "NBI Clearance uploaded successfully!";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Status500","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadMOA(IFormFile file, int profileId)
        {
            try
            {
                //check if file ay may laman
                if(file == null || file.Length == 0)
                {
                    TempData["ErrorMessage"] = "No file selected!";
                    return View();
                }

                //Valid format ng file 
                var allowedExt = new[] { ".jpg", ".jpeg", ".png" };

                //CHECK KUNG TAMA YUNG FILE Format
                var fileExt = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExt.Contains(fileExt))
                {
                    TempData["ErrorMessage"] = "Invalid file type! Only .jpg, .jpeg, and .png are allowed.";
                    return View();
                }

                //VALID MINE TYPE 
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedMimeTypes.Contains(file.ContentType))
                {
                    TempData["ErrorMessage"] = "Invalid Content type! Only JPEG and PNG images are allowed.";
                    return View();
                }

                //CHECK IF THE SIZE IS VALID
                if (file.Length > 2 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File size exceed the maximum limit of 2 MB.";
                    return View();
                }

                bool upload = await _studentRepository.UploadMOA(profileId,file,fileExt);

                if (!upload)
                {
                    TempData["ErrorUploadMOA"] = "Error uploading file, Try again!";
                    return View();
                }

                TempData["SuccessUploadMOA"] = "Succcessfully upload Memorandum of Agreement.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Status500","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file,int ProfileId)
        {
            try
            {
                //check if file is null or empty
                if (file == null || file.Length == 0)
                {
                    TempData["ErrorMessage"] = "File is missing try, please attach a file.";
                    return View();
                }

                //VALID EXTENSTIONS 
                var validExt = new[] { ".jpeg", ".png", ".jpg" };
                var fileExt = Path.GetExtension(file.FileName).ToLower();

                if (!validExt.Contains(fileExt))
                {
                    TempData["ErrorMessage"] = "Invalid file extension.";
                    return View();
                }

                //CHECK IF MIME TYPES IS VALID 
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedMimeTypes.Contains(file.ContentType))
                {
                    TempData["ErrorMessage"] = "Invalid file format.";
                    return View();
                }

                //check if the file size is valid
                if (file.Length > 2 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File exceeeded 2MB Requirements";
                    return View();
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return RedirectToAction("Status500", "Home");
            }
        }
    }
}
