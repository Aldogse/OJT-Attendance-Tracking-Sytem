using Attendace_Tracking_Sytem.Enums;

namespace Attendace_Tracking_Sytem.DTO
{
    public class studentProfileDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public int Age { get; set; }
        public Departments Department { get; set; }
        public string School { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
