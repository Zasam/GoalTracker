using System;
using System.Globalization;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool val)
                {
                    if (val)
                        return Color.Green;
                    return Color.Red;
                }
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