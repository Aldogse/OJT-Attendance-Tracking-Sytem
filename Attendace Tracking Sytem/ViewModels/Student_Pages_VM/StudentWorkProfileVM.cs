using System.ComponentModel.DataAnnotations;
using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentWorkProfileVM
    {
        [Required]
        public int? StudentProfileId { get; set; } = null;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

    }
}
