using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace Attendace_Tracking_Sytem.Models.StudentProfiles
{
    public class StudentLogInCredentials: IdentityUser
    {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
