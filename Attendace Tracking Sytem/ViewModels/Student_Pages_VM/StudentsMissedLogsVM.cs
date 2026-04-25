using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentsMissedLogsVM
    {
        public int LogId { get; set; }
        public string Fullname { get; set; }
        public DateOnly LogDate { get; set; }
        public TimeSpan? Timeout { get; set; }
        public MissedLogStatus status { get; set; }
        public int? ProfileId { get; set; }
    }
}
