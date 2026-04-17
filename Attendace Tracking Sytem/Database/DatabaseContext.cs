using Attendace_Tracking_Sytem.Models.Account;
using Attendace_Tracking_Sytem.Models.HR_Profiles;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
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

        public DbSet<StudentProfile> StudentsProfile { get; set; }
        public DbSet<HRProfile> HRProfile { get; set; }
        public DbSet<StudentLogs> StudentLogs { get; set; }
        public DbSet<MissedTimeouts> MissedTimeouts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentLogs>()
                .HasOne(i => i.StudentProfile)
                .WithMany()
                .HasForeignKey(i => i.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentLogs>()
                .HasData(
                  new StudentLogs()
                  {
                      ProfileId = 2,
                      TimeIn = new DateTime(2024, 1, 1, 8, 0, 0),
                      TimeOut = null,
                      TotalHours = 0,
                      Status = Enums.AttendanceStatus.Incomplete,
                      LogDate = new DateOnly(2026, 4, 16),
                      LogId = 2
                  }
                );

            builder.Entity<MissedTimeouts>()
                .HasData(
                   new MissedTimeouts()
                   {
                       ProfileId = 2,
                       LogDate = new DateOnly(2026, 4, 16),
                       LogId = 2,    
                       Id = 1
                   }                
                );

        }
    }
}
