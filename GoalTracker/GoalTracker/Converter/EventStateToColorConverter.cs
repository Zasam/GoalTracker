using System;
using System.Globalization;
using GoalTracker.Models;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class EventStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is CalendarEventState eventState)
                {
                    if (!eventState.Approved)
                        return Color.LightSlateGray;

                    if (eventState.Success)
                        return Color.Green;

                    return Color.Red;
                }

                return Color.Transparent;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}