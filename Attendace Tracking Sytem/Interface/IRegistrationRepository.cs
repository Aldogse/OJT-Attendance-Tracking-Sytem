using Attendace_Tracking_Sytem.Models;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.Interface
{
    public interface IRegistrationRepository
    {
        Task<StudentProfile> StudentProfileSetUp(StudentProfile studentProfile);
        Task<StudentWorkProfile> StudentWorkProfileSetUp(StudentWorkProfileVM studentWorkProfileVM);
        Task<HRProfile>HrProfileSetUp(HrProfileVM HrProfile);
    }
}
