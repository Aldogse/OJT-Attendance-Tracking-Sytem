using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Controllers
{
    public class HrController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<HrController> _logger;
        private readonly IHrRepository _hrRepository;

        public HrController(IRegistrationRepository registrationRepository,DatabaseContext databaseContext
            ,ILogger<HrController>logger,
            IHrRepository hrRepository)
        {
            _registrationRepository = registrationRepository;
            _databaseContext = databaseContext;
            _logger = logger;
            _hrRepository = hrRepository;
        }

       [HttpGet]
       public IActionResult HrProfileForm(string UserId)
       {
            var user = new HrProfileVM();
            user.UserId = UserId;
            return View(user);
       }

       [HttpPost]
       public async Task<IActionResult> HrProfileForm(HrProfileVM newProfile)
       {
            try
            {
                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input try again!");
                    return View(newProfile);
                }

                var IdExist = await _databaseContext.HRProfile.AnyAsync(i => i.EmployeeId == newProfile.EmployeeId);
                var NumExist = await _databaseContext.HRProfile.AnyAsync(i => i.ContactNumber == newProfile.ContactNumber);

                if (IdExist)
                {
                    ModelState.AddModelError("","Employee ID already exist!");
                    return View(newProfile);
                }
                else if (NumExist)
                {
                    ModelState.AddModelError("", "Contact number already exist!");
                    return View(newProfile);
                }

                var exist = await _databaseContext.HRProfile.FirstOrDefaultAsync(i => i.UserId == newProfile.UserId);

                if(exist != null)
                {
                    ModelState.AddModelError("", "User ID already exist!");
                    return View(newProfile);
                }

                var profile = await _registrationRepository.HrProfileSetUp(newProfile);

                var account = await _databaseContext.Users.Where(i => i.Id == profile.UserId).FirstOrDefaultAsync();

                account.ProfileCompleted = true;
                await _databaseContext.SaveChangesAsync();
                return RedirectToAction("HrDashBoard", "Hr", new { UserId = account.Id });

            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
       }

        [HttpGet]
        public async Task<IActionResult> HrDashBoard()
        {
            try
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ojtData = await _hrRepository.HrDashboardInformation(UserId);

                return View(ojtData);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveProfile(int Id)
        {
            try
            {
                 await _hrRepository.ApproveStudentWorkProfile(Id);
                 return RedirectToAction("HrDashboard","Hr");
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MissedLogDetails(int ProfileId)
        {
            try
            {
                
                StudentMissedLogDetailsVM? studentData = await _hrRepository.MissTimeoutDetails(ProfileId);

                return View(studentData);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error:{ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApprovedMissedLog(int ProfileId,DateOnly date)
        {
            try
            {
                string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                if(UserId == null)
                {
                    ModelState.AddModelError("","Payload Missing");
                    return View();
                }
                await _hrRepository.ApproveMissedLog(ProfileId,date,UserId);

                TempData["ApproveSuccess"] = "Successfully modifed!";

                return RedirectToAction("HrDashboard","Hr");
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error:{ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MissedLogs(MissedLogStatus? status,DateOnly? date)
        {
            var logs = await _hrRepository.StudentMissedLogsFiltered(status,date);           
            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> Students(int page = 1,Departments? department= null)
        {
            var students = await _hrRepository.GetStudents(page,department);

            ViewBag.CurrentPage = page;
            ViewBag.Pagesize = 5;
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> StudentProfile(int ProfileId)
        {
            try
            {
                var studentDetails = await _hrRepository.GetStudentProfile(ProfileId);
                return View(studentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error:{ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StudentLogsSummary(int ProfileId,int page = 1,DateOnly? StartDate = null,DateOnly? EndDate = null)
        {
            try
            {
                var StudentLog = await _hrRepository.GetStudentLogSummary(ProfileId,page,StartDate,EndDate);
                ViewBag.PageSize = 10;
                ViewBag.CurrentPage = page;
                ViewBag.ProfileId = ProfileId;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                return View(StudentLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

    }

}
 