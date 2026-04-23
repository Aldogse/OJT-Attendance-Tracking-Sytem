using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;

namespace Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM
{
    public class StudentLogSummaryVM
    {
        public string Fullname { get; set; }
        public Departments Department { get; set; }
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }
       public ICollection<StudentLogVM>? StudentLogs { get; set; }
    }
}
