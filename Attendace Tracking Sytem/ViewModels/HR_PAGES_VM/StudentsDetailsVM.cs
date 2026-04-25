using System.ComponentModel.DataAnnotations;
using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class StudentsDetailsVM
    {
        public string Fullname { get; set; }
        public string StudentId { get; set; }
        public Departments Department { get; set; }
        public decimal? HoursRendered { get; set; }
        public decimal RequiredHours { get; set; }
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
