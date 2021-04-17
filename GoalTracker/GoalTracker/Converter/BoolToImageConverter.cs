using System;
using System.Globalization;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    /// <summary>
    /// Converts boolean value to specific image source TODO: Change this to get value of enum to specify the exact image source that is needed
    /// </summary>
    public class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                if (boolValue)
                    return "Unlocked.png";

            return "Locked.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}