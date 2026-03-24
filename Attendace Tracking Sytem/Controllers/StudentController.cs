using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly DatabaseContext _databaseContext;

        public StudentController(IRegistrationRepository registrationRepository,DatabaseContext databaseContext)
        {
            _registrationRepository = registrationRepository;
            _databaseContext = databaseContext;
        }

        public IActionResult SuccessRegistrationPage()
        {
            return View();
        }

        public IActionResult StudentProfileForm()
        {
            return View();
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
    }
}
