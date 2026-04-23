using Attendace_Tracking_Sytem.Database;
using Attendace_Tracking_Sytem.Enums;
using Attendace_Tracking_Sytem.Interface;
using Attendace_Tracking_Sytem.Models.HR_RELATED_MODELS;
using Attendace_Tracking_Sytem.Models.StudentProfiles;
using Attendace_Tracking_Sytem.ViewModels.HR_DASHBOARD_VM;
using Attendace_Tracking_Sytem.ViewModels.HR_PAGES_VM;
using Attendace_Tracking_Sytem.ViewModels.Student_Pages_VM;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Tracking_Sytem.Repository
{
    public class HrRepository : IHrRepository
    {
        private readonly DatabaseContext _databaseContext;

        public HrRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task ApproveMissedLog(int ProfileId,DateOnly Logdate,string UserId) 
        {
            var log = await _databaseContext.MissedTimeouts.FirstOrDefaultAsync(i => i.ProfileId == ProfileId && i.LogDate == Logdate);
            var studentLogs = await _databaseContext.StudentLogs.Include(i => i.StudentProfile).FirstOrDefaultAsync(i => i.ProfileId == ProfileId && i.LogDate == Logdate);
            int HrProfileId = await _databaseContext.HRProfile.Where(i => i.UserId == UserId).Select(i => i.ProfileId).FirstOrDefaultAsync();

            studentLogs.TimeOut = log.Timeout;
            studentLogs.TotalHours = (decimal)(studentLogs.TimeOut - studentLogs.TimeIn)?.TotalHours;
            studentLogs.Status = Enums.AttendanceStatus.Complete;
            studentLogs.StudentProfile.HoursRendered = studentLogs.TotalHours;
            log.status = Enums.MissedLogStatus.Approved;
            log.ApproverProfileId = HrProfileId;

            await _databaseContext.SaveChangesAsync();

        }

        public async Task ApproveStudentWorkProfile(int Id)
        {
            var student = new StudentProfile { ProfileId = Id };


            _databaseContext.StudentsProfile.Attach(student);

            student.Status = Enums.Status.Active;

            _databaseContext.StudentsProfile.Entry(student)
                .Property(i => i.Status).IsModified = true;

            await _databaseContext.SaveChangesAsync();
        }

        public Task DenyStudentWorkProfile(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<HrDashBoardVM> HrDashboardInformation(string UserId)
        {
            DateTime date = DateTime.Now;
            var UserData = await _databaseContext.HRProfile.Where(i => i.UserId == UserId).FirstOrDefaultAsync();

            var pendingStatus = await _databaseContext.StudentsProfile.Where(i => i.Status == Enums.Status.Pending)
                .ToListAsync();

            var numberOfActiveStudents = await _databaseContext.StudentsProfile.Where(i => i.Status == Enums.Status.Active)
                .CountAsync();

            var NumOfFinishingStudents = await _databaseContext.StudentsProfile.Where(i => i.EndDate.Month == date.Month).CountAsync();

            return new HrDashBoardVM
            {
                NumberOfActiveStudents = numberOfActiveStudents,
                FinishingStudents = NumOfFinishingStudents,
                PendingStudents = pendingStatus,
                FullName = $"{UserData.FirstName} {UserData.MiddleName} {UserData.LastName}",
            };
        }

        //BACKGROUND SERVICE 
        public async Task<List<StudentLogs>> MissedTimeOuts(DateOnly date)
        {
            var logs = await _databaseContext.StudentLogs.Where(i => i.LogDate == date && i.TimeOut == null)
                .ToListAsync();

            return logs;
        }

        public async Task<StudentMissedLogDetailsVM> MissTimeoutDetails(int ProfileId)
        {
            var log = await _databaseContext.MissedTimeouts
                .Include(i => i.Profile).FirstOrDefaultAsync(i => i.ProfileId == ProfileId);

            var logVm = new StudentMissedLogDetailsVM
            {
                Department = log.Profile.Department,

                Fullname = log != null
                ? $"{log.Profile.FirstName} {log.Profile.MiddleName} {log.Profile.LastName}"
                : "Unknown",

                Explanation = log.Explanation,
                Logdate = log.LogDate,
                Timeout = log.Timeout,
                ProfileId = ProfileId
            };

            return logVm;
                
        }

        public async Task<List<StudentsMissedLogsVM>> StudentMissedLogsFiltered(MissedLogStatus? missedLogStatus, DateOnly? date)
        {
            var query =  _databaseContext.MissedTimeouts.Include(i => i.Profile).AsQueryable();
            List<MissedTimeouts> logs = new List<MissedTimeouts>();

            if (missedLogStatus.HasValue && date == null)
            {
                query =  query.Where(i => i.status == missedLogStatus.Value);

                logs = await query.ToListAsync();               
            }
            else if (!missedLogStatus.HasValue && date != null)
            {
                query = query.Where(i => i.LogDate == date);
                logs = await query.ToListAsync();
            }
            else if (missedLogStatus.HasValue && date != null)
            {
                query = query.Where(i => i.LogDate == date && i.status == missedLogStatus.Value);
                logs = await query.ToListAsync();
            }else
            {
                logs = await query.ToListAsync();
            }

            return logs.Select(i => new StudentsMissedLogsVM
            {
                Fullname = $"{i.Profile.FirstName} {i.Profile.MiddleName} {i.Profile.LastName}",
                LogDate = i.LogDate,
                LogId = i.LogId,
                ProfileId = i.ProfileId,
                status = i.status,
                Timeout = i.Timeout,
            })
                .OrderByDescending(i => i.LogDate)
                .ToList();
        }

        public async Task<List<StudentVM>> GetStudents(int page = 1,Departments? department = null)
        {
            int size = 5;

            var query =  _databaseContext.StudentsProfile.AsQueryable();

            if(department != null)
            {
                query = query.Where(i => i.Department == department);
            }


            return await query.Select(i => new StudentVM
            {
               ProfileId = i.ProfileId,
               Department = i.Department,
               Fullname = $"{i.FirstName} {i.MiddleName} {i.LastName}",
               HoursRendered = i.HoursRendered,
               StudentId = i.StudentId,
            })
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
            
        }

        public async Task<StudentsDetailsVM> GetStudentProfile(int ProfileId)
        {
            var student = await _databaseContext.StudentsProfile.FirstOrDefaultAsync(i => i.ProfileId == ProfileId);

            var details = student != null
                ? new StudentsDetailsVM()
                  {
                    Age = student.Age,
                    Department = student.Department,
                    Email = student.Email,
                    EndDate = student.EndDate,
                    HoursRendered = student.HoursRendered,
                    PhoneNumber = student.PhoneNumber,
                    StudentId = student.StudentId,
                    RequiredHours = student.RequiredHours,
                    ShiftEnd = student.ShiftEnd,
                    ShiftStart = student.ShiftStart,
                    StartDate = student.StartDate,
                    Fullname = $"{student.FirstName} {student.MiddleName} {student.LastName}"
                  }
                : new StudentsDetailsVM();

            return details;
           
        }

        public async Task<StudentLogSummaryVM> GetStudentLogSummary(int ProfileId, int page = 1, DateOnly? StartDate = null, DateOnly? EndDate = null)
        {
            int size = 10;
            //QUERY LAHAT NG DATA SA STUDENT LOG 
            var query = _databaseContext.StudentLogs.Where(i => i.ProfileId == ProfileId);

            //APPLY FILTER KUNG MERON
            if(StartDate != null && EndDate != null)
            {
                query = query.Where(i => i.LogDate >= StartDate && i.LogDate <= EndDate);
            }

            //CONVERT IT TO VIEW MODEL
            var logs = query.Select(i => new StudentLogVM
            {
                Logdate = i.LogDate,
                Timein = i.TimeIn.ToString(@"hh\:mm"),
                Timeout = i.TimeOut.ToString() != null
                ? i.TimeOut.Value.ToString(@"hh\:mm")
                :"--",
                TotalHours = i.TotalHours,
            })
                .Skip((page - 1) * size)
                .Take(size);

            var students = await _databaseContext.StudentsProfile.Where(i => i.ProfileId == ProfileId)
                .Select(i => new StudentLogSummaryVM
                {
                    Department = i.Department,
                    Fullname = $"{i.FirstName} {i.MiddleName} {i.LastName}",
                    ShiftEnd = i.ShiftEnd,
                    ShiftStart = i.ShiftStart,
                    StudentLogs = logs.ToList(),
                }).FirstOrDefaultAsync();

            return students ?? new StudentLogSummaryVM();
        }
    }
}
