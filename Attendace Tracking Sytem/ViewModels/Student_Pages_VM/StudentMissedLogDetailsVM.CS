using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentMissedLogDetailsVM
    {
        public string Fullname { get; set; }
        public int ProfileId { get; set; }
        public string Department { get; set; }
        public DateOnly Logdate { get; set; }

        [StringLength(250)]
        public string Explanation { get; set; }
        public TimeSpan? Timeout { get; set; }
    }
}
