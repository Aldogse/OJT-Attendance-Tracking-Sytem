using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class StudentVM
    {
        public int ProfileId { get; set; }
        public string Fullname { get; set; }
        public string StudentId { get; set; }
        public Departments Department { get; set; }
        public decimal? HoursRendered { get; set; }
    }
}
