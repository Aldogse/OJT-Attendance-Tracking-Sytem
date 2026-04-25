namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentLogVM
    {
        public DateOnly Logdate { get; set; }
        public string Timein { get; set; }
        public string Timeout { get; set; }
        public decimal? TotalHours { get; set; }
    }
}
