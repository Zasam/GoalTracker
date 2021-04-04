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
                    return (long) (value - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime()).TotalMilliseconds;

                return (long) (value.ToLocalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime()).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        /// <summary>
        ///     Returns the specified date added by one hour, and minutes and seconds are flatten by 0
        /// </summary>
        /// <param name="value">The passed datetime value</param>
        /// <returns>The new datetime with flatten values for minutes/seconds</returns>
        public static DateTime AddHourAndFlattenTime(this DateTime value)
        {
            var newDate = value.AddHours(1);
            return new DateTime(newDate.Year, newDate.Month, newDate.Day, newDate.Hour, 0, 0);
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        // Solution from: https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
        public static int GetWeekOfYear(this DateTime time)
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
    }
}