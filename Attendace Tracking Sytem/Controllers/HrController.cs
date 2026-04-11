using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
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

                var profile = await _registrationRepository.HrProfileSetUp(newProfile);
                return RedirectToAction("Index");

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
                StudentMissedLogDetailsVM? studentData = await _databaseContext.MissedTimeouts.Where(i => i.ProfileId == ProfileId)
                    .Select(i => new StudentMissedLogDetailsVM
                    {
                        Department = i.Profile.Department,
                        Fullname = $"{i.Profile.FirstName} {i.Profile.MiddleName} {i.Profile.LastName}",
                        Logdate = i.LogDate.ToShortDateString(),
                        ProfileId = ProfileId
                    }).FirstOrDefaultAsync();

                return View(studentData);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error:{ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }
    }
}
