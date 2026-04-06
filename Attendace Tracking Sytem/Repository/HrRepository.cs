using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Repository
{
    public class HrRepository : IHrRepository
    {
        private readonly DatabaseContext _databaseContext;

        public HrRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task ApproveStudentWorkProfile(int Id)
        {
            var student = new StudentWorkProfile { Id = Id };


            _databaseContext.StudentsWorkProfile.Attach(student);

            student.Status = Enums.Status.Active;

            _databaseContext.StudentsWorkProfile.Entry(student)
                .Property(i => i.Status).IsModified = true;

            await _databaseContext.SaveChangesAsync();
        }

        public Task DenyStudentWorkProfile(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<HrDashBoardVM> HrDashboardInformation(string UserId)
        {
            DateTime date = DateTime.Now;
            var UserData = await _databaseContext.HRProfile.Where(i => i.UserId == UserId).FirstOrDefaultAsync();

            var pendingStatus = await _databaseContext.StudentsWorkProfile.Include(i => i.StudentProfile).Where(i => i.Status == Enums.Status.Pending)
                .ToListAsync();

            var numberOfActiveStudents = await _databaseContext.StudentsWorkProfile.Where(i => i.Status == Enums.Status.Active)
                .CountAsync();

            var NumOfFinishingStudents = await _databaseContext.StudentsWorkProfile.Where(i => i.EndDate.Month == date.Month).CountAsync();

            return new HrDashBoardVM
            {
                NumberOfActiveStudents = numberOfActiveStudents,
                FinishingStudents = NumOfFinishingStudents,
                PendingStudents = pendingStatus,
                FullName = $"{UserData.FirstName} {UserData.MiddleName} {UserData.LastName}"
            };
        }
    }
}
