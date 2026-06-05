namespace Attendace_Tracking_Sytem.Helpers
{
    public class WeekDaysCounter
    {
        public static int WeeekDayCounter(DateTime date)
        {
            int weekDayCounter = 0;

            int daysInAMonth = DateTime.DaysInMonth(date.Year,date.Month);

            for(int day = 1; day <= daysInAMonth; day++)
            {
                var d = new DateTime(date.Year,date.Month,day);

                if(d.DayOfWeek != DayOfWeek.Saturday || d.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekDayCounter++;
                }

            }
            return weekDayCounter;
        }
    }
}
