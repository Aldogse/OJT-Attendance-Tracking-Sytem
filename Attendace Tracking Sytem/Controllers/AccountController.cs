using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Attendace_Tracking_Sytem.ViewModels.Account_Pages_VM;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Attendace_Tracking_Sytem.Database;

namespace Attendace_Tracking_Sytem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<LogInCredentials> _userManager;
        private readonly SignInManager<LogInCredentials> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly DatabaseContext _databaseContext;

        public AccountController(UserManager<LogInCredentials> userManager,
            SignInManager<LogInCredentials> signInManager,
            ILogger<AccountController>logger,
            DatabaseContext databaseContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult StudentRegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentRegistrationForm(AccountRegistrationVM accountRegistrationVM)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input try again!");
                    return View(accountRegistrationVM);
                }

                var newUser = new LogInCredentials()
                {
                    Email = accountRegistrationVM.Email,
                    UserName = accountRegistrationVM.Email
                };

                var newUserCredentials = await _userManager.CreateAsync(newUser,accountRegistrationVM.Password);

                if (newUserCredentials.Succeeded)
                {

                    var role = await _userManager.AddToRoleAsync(newUser,Enums.Roles.Student.ToString());
                    return RedirectToAction("StudentProfileForm", "Student",new { UserId = newUser.Id});
                }

                foreach (var error in newUserCredentials.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }

                return View(accountRegistrationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("SystemError");
            }
        }

        //HR REGISTRATION PROCESS
        [HttpGet]
        public IActionResult HrRegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HrRegistrationForm(AccountRegistrationVM accountRegistrationVM)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input!");
                    return View(accountRegistrationVM);
                }

                var newUser = new LogInCredentials()
                {
                    UserName = accountRegistrationVM?.Email,
                    Email = accountRegistrationVM?.Email,
                };

                var newUserCredentials = await _userManager.CreateAsync(newUser,accountRegistrationVM.Password);

                if (newUserCredentials.Succeeded)
                {
                    var role = await _userManager.AddToRoleAsync(newUser,Roles.HR.ToString());
                    return RedirectToAction("HrProfileForm","Hr",new {UserId = newUser.Id});
                }

                if (!newUser.ProfileCompleted)
                {
                    return RedirectToAction("HrProfileForm", "Hr",new {UserId = newUser.Id});
                }

                foreach (var err in newUserCredentials.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

                return View(accountRegistrationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("InvalidOperation","Home");
            }
        }

        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPage(LoginVM loginCredentials)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input!");
                    return View(loginCredentials);
                }

                var user = await _userManager.FindByEmailAsync(loginCredentials.EmailAddress);
                
                if(user == null)
                {
                    ModelState.AddModelError("","Profile not found!");
                    return View(loginCredentials);
                }

                if (!user.ProfileCompleted)
                {
                    var hr = await _databaseContext.HRProfile.FirstOrDefaultAsync(i => i.UserId == user.Id);

                    if (hr != null)
                    {
                        return RedirectToAction("HrProfileForm", "Hr", new { UserId = user.Id });
                    }

                    return RedirectToAction("StudentProfileForm","Student",new {UserId = user.Id});
                }

                var result = await _signInManager.PasswordSignInAsync(user.Email!, loginCredentials.Password,false,false);

                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, "Student"))
                    {
                        //check if the profile has been approved, if not route success page
                        var profile = await _databaseContext.StudentsProfile.Where(i => i.UserId == user.Id)
                            .Select(i => i.Status).FirstOrDefaultAsync();

                        if (profile.Equals(Status.Pending))
                        {
                            return RedirectToAction("Success","Student");
                        }

                        return RedirectToAction("StudentDashboard", "Student",new {UserId = user.Id});
                    }
                    else if (await _userManager.IsInRoleAsync(user, "HR"))
                    {
                        return RedirectToAction("HrDashboard", "Hr",new {UserId = user.Id});
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Supervisor"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("","Invalid Email or Password");
                return View(loginCredentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction();
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            try
            {
               await _signInManager.SignOutAsync();
               return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                return RedirectToAction("Error","Home");
            }
        }
    }
}

