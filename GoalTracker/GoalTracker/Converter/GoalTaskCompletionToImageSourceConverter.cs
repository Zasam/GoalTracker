using System;
using System.Globalization;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class GoalTaskCompletionToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool goalTaskCompleted)
                    if (goalTaskCompleted)
                        return "Failed.png";

                return "Success.png";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return "Failed.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}