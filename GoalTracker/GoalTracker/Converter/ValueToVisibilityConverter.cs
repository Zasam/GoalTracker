using System;
using System.Globalization;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class ValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return false;

                if (value is bool boolValue)
                    return boolValue;

                if (value is string stringValue)
                {
                    if (string.IsNullOrWhiteSpace(stringValue))
                        return false;

                    return true;
                }

                throw new InvalidOperationException(
                    "Type for convertion in ValueToVisibilityConverter is not defined. Add type for " + value.GetType() + "in code...!");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}