using System.ComponentModel.DataAnnotations;
using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentPendingWorkProfileVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Department { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }
            
        [Required]
        public DateOnly EndDate { get; set; }

        public decimal? HoursRendered { get; set; }
        public decimal RequiredHours { get; set; } = 300;
        public TimeOnly ShiftStart { get; set; } = new TimeOnly(8, 0);
        public TimeOnly ShiftEnd { get; set; } = new TimeOnly(17, 0);
        public Status Status { get; set; } = Status.Pending;
    }
}
