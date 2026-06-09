
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Attendace_Tracking_Sytem.Services
{
    public class AbsentHrNotification : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AbsentHrNotification> _logger;
        private int errCount;

        public AbsentHrNotification(IServiceScopeFactory serviceScopeFactory,ILogger<AbsentHrNotification>logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                errCount++;

                if (errCount >= 5)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailServices>();
                    emailService.sendEmailAsync("ezekiellamoste4@gmail.com", "Failing Service",
                    $"<h1>AbsentHrNotification hit maximum retries but failed to execute! Error Message:{ex.Message} <h1>");
                }

                _logger.LogError(message: $"Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }

        private async Task AbsentNotificationFunction(IHrRepository hrRepository)
        {
            var absentees = await hrRepository.GetStudentWith3absencesOrMore();

            if(absentees == null)
            {
                _logger.LogInformation(message:$"No student has 3 or more absent for {DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year}");
                return;
            }
        }
    }
}
