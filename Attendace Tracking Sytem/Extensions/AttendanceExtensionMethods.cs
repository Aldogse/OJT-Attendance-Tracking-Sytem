namespace Attendace_Tracking_Sytem.Helpers
{
    public static class AttendanceExtensionMethods
    {
        public static int WeeekDayCounter(this DateTime date)
        {
            int weekDayCounter = 0;

            int daysInAMonth = DateTime.DaysInMonth(date.Year,date.Month);

            for(int day = 1; day <= daysInAMonth; day++)
            {
                var d = new DateTime(date.Year,date.Month,day);

                if(d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekDayCounter++;
                }

            }
            return weekDayCounter;
        }

        public static double ToPercentageOf(this int value,int weekDays)
        {
            if(weekDays == 0)
                return 0;

            return ((double)value / weekDays) * 100;
        }

        public static double ToPercentage(this int value,int weekDays)
        {
            if(weekDays == 0)
                return 0;

            return ((double)value / weekDays) * 100;
        }
    }
}
