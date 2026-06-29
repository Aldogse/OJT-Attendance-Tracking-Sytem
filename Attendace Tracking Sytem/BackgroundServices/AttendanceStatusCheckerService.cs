using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Services;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Attendace_Tracking_Sytem.BackgroundServices
{
    public class AttendanceStatusCheckerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceStatusCheckerService> _logger;
        private int errCount;
        public AttendanceStatusCheckerService(IServiceScopeFactory serviceScopeFactory, ILogger<AttendanceStatusCheckerService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailServices>();
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var repository = scope.ServiceProvider.GetRequiredService<IStudentRepository>();

                    //CHECK AT 6PM 
                    DateTime now = DateTime.UtcNow;
                    DateTime nextRun = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);

                    if (now.TimeOfDay > nextRun.TimeOfDay)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;

                    await AttendanceService(database, repository);
                    errCount = 0;
                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {

                    errCount++;

                    if (errCount >= 5)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailServices>();
                        emailService.sendEmailAsync("ezekiellamoste4@gmail.com", "Failing Service",
                        $"<h1>Attendance Status Checker Service hit maximum retries but failed to execute! Error Message:{ex.Message} <h1>");
                    }
                    _logger.LogError(message: $"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);

                }
            }
        }

        //ATTENDANCE CHECKER
        private async Task AttendanceService(DatabaseContext databaseContext, IStudentRepository studentRepository)
        {
            var now = DateTime.UtcNow;
            var logDate = new DateOnly(now.Year, now.Month, now.Day);

            var attendanceReport = await studentRepository.GetDailyAttendanceReport(logDate);

            await databaseContext.DailyAttendanceReports.AddAsync(attendanceReport);
            await databaseContext.SaveChangesAsync();
        }
    }
}
