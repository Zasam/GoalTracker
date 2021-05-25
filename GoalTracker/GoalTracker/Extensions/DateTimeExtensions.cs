using System;
using System.Globalization;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Extensions
{
    public static class DateTimeExtensions
    {
        public static long GetMillisecondsSince1970(this DateTime value, bool isLocalTime = true)
        {
            try
            {
                if (isLocalTime)
                {
                    var date1970 = new DateTime(1970, 1, 1, 0, 0, 0);
                    return (long) (value - date1970).TotalMilliseconds - 1000 * 60 * 60 * 2; //TODO: Why do I have to remove 2 hours to get the correct local time (DE-de)?
                }

                var date1970Local = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
                return (long) (value.ToLocalTime() - date1970Local).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        // Solution from: https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
        public static int GetWeekOfYear(this DateTime time)
        {
            try
            {
                // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
                // be the same week# as whatever Thursday, Friday or Saturday are,
                // and we always get those right
                var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                    time = time.AddDays(3);

                // Return the week of our adjusted day
                return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }
    }
}