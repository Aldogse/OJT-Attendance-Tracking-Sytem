
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Repository;
using Microsoft.AspNetCore.Http.Extensions;

namespace Attendace_Tracking_Sytem.Services
{
    public class LogCheckBackgroundService : BackgroundService
    {
        private readonly ILogger<LogCheckBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScope;

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
                    DateTime now = DateTime.Now;
                    DateTime nextDay = now.Date.AddDays(1);

                    var delay = nextDay - now;
                    await LogBackgroundService(repository, database);
                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(message: $"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                }
            }
        }

        private async Task LogBackgroundService(IHrRepository hrRepository,DatabaseContext database)
        {
            try
            {
                //this should have .adddays + 1
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
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return;
            }
        }
    }
}
