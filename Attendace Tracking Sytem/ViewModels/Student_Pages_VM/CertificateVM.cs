 using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class CertificateVM
    {
        public string FullName { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public Departments Department { get; set; }
        public TimeSpan? HoursRendered { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
