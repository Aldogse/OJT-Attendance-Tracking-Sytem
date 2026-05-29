using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace Attendace_Tracking_Sytem.Models.Account
{
    public class LogInCredentials: IdentityUser
    {
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool ProfileCompleted { get; set; } = false; 
    }
}
