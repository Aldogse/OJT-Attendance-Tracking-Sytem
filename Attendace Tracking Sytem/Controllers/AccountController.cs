using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Attendace_Tracking_Sytem.ViewModels;

namespace Attendace_Tracking_Sytem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser>userManager,
            SignInManager<IdentityUser>signInManager,
            ILogger<AccountController>logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult StudentRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentRegistration(AccountRegistrationVM accountRegistrationVM)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Invalid Input try again!");
                    return View(accountRegistrationVM);
                }

                var newUser = new IdentityUser()
                {
                    Email = accountRegistrationVM.Email,
                    UserName = accountRegistrationVM.Email
                };

                var newUserCredentials = await _userManager.CreateAsync(newUser,accountRegistrationVM.Password);

                if (newUserCredentials.Succeeded)
                {
                    var role = await _userManager.AddToRoleAsync(newUser,Enums.Roles.Student.ToString());
                    return RedirectToAction("StudentProfile",new { UserId = newUser.Id});
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
    }
}
