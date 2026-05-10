namespace Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM
{
    public class StudentRequirementsPageVM
    {
        public string? NBIImagePath { get; set; } 
        public string? MemorandumOfAgreementImagePath { get; set; }
        public string? StudentIdImagePath { get; set; }

        public IFormFile? NBIFile { get; set; }
        public IFormFile? MOAFile { get; set; }
        public IFormFile? StudentIdFile { get; set; }

        public int ProfileId { get; set; }
    }
}
