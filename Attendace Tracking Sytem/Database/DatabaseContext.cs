using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Database
{
    public class DatabaseContext : IdentityDbContext<StudentLogInCredentials>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<StudentWorkProfile> StudentsWorkProfile{ get; set; }
        public DbSet<StudentProfile> StudentsProfile { get; set; }
        public DbSet<StudentLogInCredentials> StudentLogInCredentials { get; set; }
    }
}
