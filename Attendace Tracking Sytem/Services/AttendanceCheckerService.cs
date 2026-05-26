
using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Services
{
    public class AttendanceCheckerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceCheckerService> _logger;

        public AttendanceCheckerService(IServiceScopeFactory serviceScopeFactory,ILogger<AttendanceCheckerService>logger)
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
                    //CURRENT TIME 
                   DateTime now = DateTime.UtcNow;
                    
                   //5 PM TIME  
                    var nextRun = new DateTime(
                        now.Year,
                        now.Month,
                        now.Day,
                        17,
                        0,
                        0
                    );

                    //KAPAG ANG START UP AY MORE THAN 5PM NA 
                    //ADJUST NG 1 SA NEXT RUN
                    if(now.TimeOfDay > nextRun.TimeOfDay)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;
                    await Task.Delay(delay, stoppingToken);

                    //CHECK IF THE DATA IS MONDAY TO FRIDAY
                    if (now.DayOfWeek != DayOfWeek.Saturday
                        && now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                        var repository = scope.ServiceProvider.GetRequiredService<IStudentRepository>();
                        await AttendanceChecker(database, repository);
                    }                
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }

        //LOGIC
        private async Task AttendanceChecker(DatabaseContext databaseContext,IStudentRepository studentRepository)
        {
            //kunin ang date today
            DateOnly date = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day));

            //GET ALL PROFILE ID FROM STUDENT PROFILE 
            var profileIds = await studentRepository.GetProfileIds();

            //KUNIN LAHAT NG PROFILE ID SA STUDENT LOGS 
            var studentLogProfileId = await databaseContext.StudentLogs.Where(i => i.LogDate == date)
                .Select(i => i.ProfileId)
                .ToHashSetAsync();
                

            //DITO IISTORE AND MGA MISSED LOGS PARA I DEFAULT 
            List<StudentLogs> defaultLogs = [];

            //Iterate ka sa profile id para icheck ano profile id and hindi nag punch in para ma set sa default after 5pm
            foreach(var id in profileIds)
            {
                if (!studentLogProfileId.Contains(id))
                {
                    //create ng default log 
                    var logs = new StudentLogs
                    {
                        isAbsent = true,
                        LogDate = date,
                        ProfileId = id,
                        TimeIn = TimeSpan.Zero,
                        TimeOut = TimeSpan.Zero,
                        TotalHours = TimeSpan.Zero,
                        Status = Enums.AttendanceStatus.Complete,                      
                    };

                    //i store sa list na ginawa
                    defaultLogs.Add(logs);
                }
            }

            //i add ng isang bagsakan sa database 
            await databaseContext.StudentLogs.AddRangeAsync(defaultLogs);
            await databaseContext.SaveChangesAsync();
        }
    }
}
