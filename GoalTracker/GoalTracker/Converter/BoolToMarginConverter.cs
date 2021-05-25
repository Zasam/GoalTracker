using System;
using System.Globalization;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class BoolToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool boolValue)
                {
                    if (boolValue)
                        return new Thickness(0);

                    return new Thickness(0, -15, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}