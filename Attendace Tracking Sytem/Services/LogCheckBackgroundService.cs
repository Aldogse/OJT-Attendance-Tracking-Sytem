
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Repository;

namespace Attendace_Tracking_Sytem.Services
{
    public class LogCheckBackgroundService : BackgroundService
    {
        private readonly ILogger<LogCheckBackgroundService> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly DatabaseContext _databaseContext;

        public LogCheckBackgroundService(ILogger<LogCheckBackgroundService>logger
            ,IStudentRepository studentRepository,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _databaseContext = databaseContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                _logger.LogError(message:$"Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromHours(2),stoppingToken);
            }
        }

        private async Task LogBackgroundService(StudentRepository studentRepository)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error: {ex.Message}");
                return;
            }
        }
    }
}
