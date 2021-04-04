using System;
using System.Globalization;
using GoalTracker.Resources;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool val)
            {
                if (val)
                    return TranslationResources_DE.TrueString;

                return TranslationResources_DE.FalseString;
            }

            return TranslationResources_DE.NullBooleanString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}