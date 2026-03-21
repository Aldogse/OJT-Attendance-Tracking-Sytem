using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels;
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
