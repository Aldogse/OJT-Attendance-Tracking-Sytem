using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Database
{
    public class DatabaseContext : IdentityDbContext<LogInCredentials>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<StudentWorkProfile> StudentsWorkProfile{ get; set; }
        public DbSet<StudentProfile> StudentsProfile { get; set; }
        public DbSet<HRProfile> HRProfile { get; set; }
        public DbSet<StudentLogs> StudentLogs { get; set; }
    }
}
