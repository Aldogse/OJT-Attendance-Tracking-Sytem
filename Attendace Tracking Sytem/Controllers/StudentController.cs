using System.Security.Claims;
using System.Threading.Tasks;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public IActionResult StudentProfileForm(string UserId)
        {
            var studentProfile = new StudentProfile();
            studentProfile.UserId = UserId;
            return View(studentProfile);
        }

        [HttpPost]
        public async Task<IActionResult> StudentProfileForm(StudentProfile StudentProfile)
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
                return RedirectToAction("StudentWorkProfileForm",new { StudentProfileId = newStudent.ProfileId});              
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        public IActionResult StudentWorkProfileForm(int StudentProfileId)
        {
            StudentWorkProfileVM student = new StudentWorkProfileVM();
            student.StudentProfileId = StudentProfileId;
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> StudentWorkProfileForm(StudentWorkProfileVM studentWorkProfileVM)
        {
            try
            {
                DateOnly currDate = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day));
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Submission Try again!");
                    return View(studentWorkProfileVM);
                }

                if(studentWorkProfileVM.StartDate == studentWorkProfileVM.EndDate)
                {
                    ModelState.AddModelError("","Start date and End date cannot be the same.");
                    return View(studentWorkProfileVM);
                }
                else if (studentWorkProfileVM.EndDate < studentWorkProfileVM.StartDate)                  
                {
                    ModelState.AddModelError("","End Date cannot be greater than Start Date!");
                    return View(studentWorkProfileVM);
                }
                else if (studentWorkProfileVM.StartDate < currDate || studentWorkProfileVM.EndDate < currDate)
                {
                    ModelState.AddModelError("","Dates cannot be in the past!");
                    return View(studentWorkProfileVM);
                }

                var newStudentWorkProfile = await _registrationRepository.StudentWorkProfileSetUp(studentWorkProfileVM);               
                return RedirectToAction("ProfileSuccess");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> HrPendingStudentDetails(int Id)
        {
            try
            {
                var student = await _studentRepository.PendingStudentWorkProfile(Id);

                var studentVM = new StudentPendingWorkProfileVM()
                {
                    Department = student.Department,
                    EndDate = student.EndDate,
                    FullName = $"{student.StudentProfile.FirstName} {student.StudentProfile.MiddleName} {student.StudentProfile.LastName}",
                    HoursRendered = student.HoursRendered,
                    RequiredHours = student.RequiredHours,
                    ShiftEnd = student.ShiftEnd,
                    ShiftStart = student.ShiftStart,
                    StartDate = student.StartDate,
                    Status = student.Status,
                    Id = Id
                };

                return View(studentVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
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

                //GET STUDENT PROFILE ID 
                var profile = await _databaseContext.StudentsWorkProfile.AsNoTracking().Where(i => i.StudentProfileId == student.ProfileId)
                    .FirstOrDefaultAsync();

                await _studentRepository.ClockIn(profile.Id);
                return RedirectToAction("StudentDashboard","Student");
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
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

                var profile = await _databaseContext.StudentsWorkProfile.AsNoTracking()
                    .Where(i => i.StudentProfileId == student.ProfileId)
                    .FirstOrDefaultAsync();

                await _studentRepository.ClockOut(profile.Id);        

                await _databaseContext.SaveChangesAsync();
                return RedirectToAction("StudentDashboard","Student");              
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }

        //STUDENT DASHBOARD
        public async Task<IActionResult> StudentDashboard()
        {
            try
            {
                var UserData = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(UserData == null)
                {
                    return RedirectToAction("Account", "LoginPage");
                }

                var Student = await _studentRepository.GetStudentDashboardData(UserData);
                return View(Student);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }
    }
}
