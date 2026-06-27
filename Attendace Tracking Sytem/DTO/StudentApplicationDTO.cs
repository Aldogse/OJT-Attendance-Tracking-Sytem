namespace Attendace_Tracking_Sytem.DTO
{
    public class StudentApplicationDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public IFormFile? Resume { get; set; } 
        public int Year { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
