using System;
using System.Globalization;
using GoalTracker.Resources;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null && value is bool val)
                {
                    if (val)
                        return TranslationResources_DE.TrueString;

                    return TranslationResources_DE.FalseString;
                }

                return TranslationResources_DE.NullBooleanString;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return TranslationResources_DE.NullBooleanString;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}