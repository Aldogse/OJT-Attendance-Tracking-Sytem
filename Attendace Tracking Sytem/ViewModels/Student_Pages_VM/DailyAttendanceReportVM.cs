namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class DailyAttendanceReportVM
    {
        public string start {  get; set; } = string.Empty;
        public string end { get; set; } = string.Empty;
        public int? numberOfAbsents { get; set; }
        public int? numberOfLates { get; set; }
        public int? numberOfPresent { get; set; }
    }
}
