using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Repository
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly DatabaseContext _databaseContext;

        public RegistrationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        //HR QUERIES
        public async Task<HRProfile> HrProfileSetUp(HrProfileVM HrProfile)
        {
            var newProfile = new HRProfile()
            {
                Address = HrProfile.Address,
                ContactNumber = HrProfile.ContactNumber,
                FirstName = HrProfile.FirstName,
                EmployeeId = HrProfile.EmployeeId,
                LastName = HrProfile.LastName,
                MiddleName = HrProfile.MiddleName,
                UserId = HrProfile.UserId,
            };

            await _databaseContext.HRProfile.AddAsync(newProfile);
            await _databaseContext.SaveChangesAsync();
            return newProfile;
        }

        //STUDENTS QUERIES
        public async Task<StudentProfile> StudentProfileSetUp(StudentProfile studentProfile)
        {
            var StudentOjtProfile = new StudentProfile()
            {
                StudentId = studentProfile.StudentId,
                Age = studentProfile.Age,
                Email = studentProfile.Email,
                FirstName = studentProfile.FirstName,
                LastName = studentProfile.LastName,
                MiddleName = studentProfile.MiddleName,
                PhoneNumber = studentProfile.PhoneNumber,
                ProfileId = studentProfile.ProfileId,
                School = studentProfile.School,
                UserId = studentProfile.UserId,
            };

            await _databaseContext.StudentsProfile.AddAsync(StudentOjtProfile);
            await _databaseContext.SaveChangesAsync();
            return StudentOjtProfile;
        }

        public async Task<StudentWorkProfile> StudentWorkProfileSetUp(StudentWorkProfileVM studentWorkProfileVM)
        {
            var StudentOjtWorkProfile = new StudentWorkProfile()
            {
                StudentProfileId = studentWorkProfileVM.StudentProfileId,
                Department = studentWorkProfileVM.Department,
                StartDate = studentWorkProfileVM.StartDate,
                EndDate = studentWorkProfileVM.EndDate,           
            };

            await _databaseContext.StudentsWorkProfile.AddAsync(StudentOjtWorkProfile);
            await _databaseContext.SaveChangesAsync();
            return StudentOjtWorkProfile;
        }


    }
}
