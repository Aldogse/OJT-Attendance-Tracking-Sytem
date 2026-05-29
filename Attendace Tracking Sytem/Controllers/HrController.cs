using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Attendace_Tracking_Sytem.ApiSettings;
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.Services;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Attendace_Tracking_Sytem.Controllers
{
    public class HrController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<HrController> _logger;
        private readonly IHrRepository _hrRepository;
        private readonly EmailServices _emailServices;

        public HrController(IRegistrationRepository registrationRepository,DatabaseContext databaseContext
            ,ILogger<HrController>logger,
            IHrRepository hrRepository,
            EmailServices emailServices)
        {
            _registrationRepository = registrationRepository;
            _databaseContext = databaseContext;
            _logger = logger;
            _hrRepository = hrRepository;
            _emailServices = emailServices;
        }

       [HttpGet]
       public IActionResult HrProfileCreated()
       {
            return View();
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

            if (account != null)
            {
                account.ProfileCompleted = true;
                await _databaseContext.SaveChangesAsync();
                return RedirectToAction("Hr", "HrProfileCreated");
                
            }

            ModelState.AddModelError("","User cannot be found!");
            return View(newProfile);


       }

        [HttpGet]
        public async Task<IActionResult> HrDashBoard()
        {

            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
            {
                return RedirectToAction("Status500","Home");
            }

            var ojtData = await _hrRepository.HrDashboardInformation(UserId);

            return View(ojtData);

        }

        [HttpPost]
        public async Task<IActionResult> ApproveProfile(int Id)
        {
            var userId = await _databaseContext.StudentsProfile.Where(i => i.ProfileId == Id)
                .Select(i => i.UserId).FirstOrDefaultAsync();

            var userEmail = await _databaseContext.Users.AsNoTracking().Where(i => i.Id == userId)
                .Select(i => i.Email).FirstOrDefaultAsync();

            if(userEmail == null)
            {
                ModelState.AddModelError("","Email cannot be found");
                return View();
            }

            await _hrRepository.ApproveStudentWorkProfile(Id);

             _emailServices.sendEmailAsync(userEmail,"Approval Message",
                "<h1>Hi your profile has bee successfully approved<h1>");


            return RedirectToAction("HrDashboard","Hr");
        }

        [HttpGet]
        public async Task<IActionResult> MissedLogDetails(int ProfileId)
        {

                StudentMissedLogDetailsVM? studentData = await _hrRepository.MissTimeoutDetails(ProfileId);

                return View(studentData);

        }

        [HttpPost]
        public async Task<IActionResult> ApprovedMissedLog(int ProfileId,DateOnly date)
        {

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if(UserId == null)
            {
                ModelState.AddModelError("","Payload Missing");
                return View();
            }

            var email = await _databaseContext.Users.AsNoTracking().Where(i => i.Id == UserId)
            .Select(i => i.Email).FirstOrDefaultAsync();

            if (email == null)
            {
                ModelState.AddModelError("", "Email not found");
                return View();
            }
            await _hrRepository.ApproveMissedLog(ProfileId,date,UserId);

             _emailServices.sendEmailAsync(email,"Missed Log Approval",
                "<h1>Missed log has been approved<h1>");

                TempData["ApproveSuccess"] = "Successfully modifed!";

                return RedirectToAction("HrDashboard","Hr");

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

                var studentDetails = await _hrRepository.GetStudentProfile(ProfileId);
                return View(studentDetails);
            

        }

        [HttpGet]
        public async Task<IActionResult> StudentLogsSummary(int ProfileId,int page = 1,DateOnly? StartDate = null,DateOnly? EndDate = null)
        {
                var StudentLog = await _hrRepository.GetStudentLogSummary(ProfileId,page,StartDate,EndDate);
                ViewBag.PageSize = 10;
                ViewBag.CurrentPage = page;
                ViewBag.ProfileId = ProfileId;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                return View(StudentLog);            
        }

        [HttpGet]
        public async Task<IActionResult> StudentRequirements(int page = 1)
        {
            var students = await _hrRepository.GetStudentRequirementsVMs(page);
            ViewBag.Pagesize = 10;
            ViewBag.CurrentPage = page;

            return View(students);
        }

        [HttpPost("{profileId:int}")]
        public async Task<IActionResult> StudentRequirementsMessage(int profileId,string message)
        {
            var req = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == profileId);

            if(req == null)
            {
                TempData["Error"] = "Issue with adding the message, Profile id is missing";
                return RedirectToAction(nameof(StudentRequirements));
            }

           req.Message = message;
           await _databaseContext.SaveChangesAsync();

            TempData["Success"] = "Message has been added!";
            return RedirectToAction(nameof(StudentRequirements));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyStudentRequirements(int profileId)
        {
            var req = await _databaseContext.StudentRequirements.FirstOrDefaultAsync(i => i.StudentProfileId == profileId);

            if (req == null)
            {
                TempData["VerifyError"] = "Issue Verifying the documents, Profile id is missing";
                return RedirectToAction(nameof(StudentRequirements));
            }

            var user = await _databaseContext.StudentsProfile.Where(i => i.ProfileId == profileId).Select(i => i.UserId).FirstOrDefaultAsync();

            var email = await _databaseContext.Users.Where(i => i.Id == user).AsNoTracking().Select(i => i.Email).FirstOrDefaultAsync();

            if(email == null)
            {
                ModelState.AddModelError("","No email attached on the account!");
                return View();
            }
            _emailServices.sendEmailAsync(email,"Verified Documents","<h1>Documents has been approved!<h1>");


            req.Verified = true;
            await _databaseContext.SaveChangesAsync();
            TempData["VerifySuccess"] = "Documents Verified!";
            return RedirectToAction(nameof(StudentRequirements));
        }
    }

}
 