using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Repository;
using Attendace_Tracking_Sytem.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Attendace_Tracking_Sytem.BackgroundServices
{
    public class LogCheckBackgroundService : BackgroundService
    {
        private readonly ILogger<LogCheckBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScope;
        private int errCount;

        public LogCheckBackgroundService(ILogger<LogCheckBackgroundService>logger,
            IServiceScopeFactory serviceScope)
        {
            _logger = logger;
            _serviceScope = serviceScope;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScope.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IHrRepository>();
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    //RUN EVERY 6PM 
                    DateTime now = DateTime.UtcNow;
                    DateTime nextRun = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);

                    if (now.TimeOfDay > nextRun.TimeOfDay)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;

                    await LogBackgroundService(repository, database);
                    errCount = 0;
                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    errCount++;

                    if (errCount >= 5)
                    {
                        using var scope = _serviceScope.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailServices>();
                        emailService.sendEmailAsync("ezekiellamoste4@gmail.com", "Failing Service",
                        $"<h1>Log Check Background Service hit maximum retries but failed to execute! Error Message:{ex.Message} <h1>");
                    }

                    _logger.LogError(message: $"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);

                }
            }
        }

        private async Task LogBackgroundService(IHrRepository hrRepository,DatabaseContext database)
        {
                
                DateOnly date = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

                var missedTimeOuts = await hrRepository.MissedTimeOuts(date);

                if(missedTimeOuts == null || missedTimeOuts.Count == 0)
                {
                    _logger.LogInformation(message:$"No missed logs for {date.Month}/{date.Year}");
                    return;
                }

                var MissingLogs = missedTimeOuts.Select(i => new MissedTimeouts
                {
                    LogDate = i.LogDate,
                    LogId = i.LogId,
                    ProfileId = i.ProfileId,
                }).ToList();

                await database.MissedTimeouts.AddRangeAsync(MissingLogs);
                await database.SaveChangesAsync();
            
        }
    }
}
