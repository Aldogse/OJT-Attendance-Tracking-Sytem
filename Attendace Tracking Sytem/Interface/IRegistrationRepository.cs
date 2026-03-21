using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IRegistrationRepository
    {
        Task<StudentProfile> StudentProfileSetUp(StudentProfile studentProfile);
        Task<StudentWorkProfile> StudentWorkProfileSetUp(StudentWorkProfileVM studentWorkProfileVM);
    }
}
