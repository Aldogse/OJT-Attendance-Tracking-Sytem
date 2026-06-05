using System.ComponentModel.DataAnnotations;

namespace Attendace_Tracking_Sytem.Models.HR_Profiles
{
    public class DailyAttendanceReport
    {
        [Key]
        public int Id { get; set; }
        public int? numberOfAbsents { get; set; }
        public int? numberOfLates { get; set; }
        public int? numberOfPresent {  get; set; }
        public DateOnly attendanceDate { get; set; }
    }
}
