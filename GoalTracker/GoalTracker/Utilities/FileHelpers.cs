using System;
using System.IO;
using System.Reflection;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Utilities
{
    public static class FileHelpers
    {
        public static string GetFileTextInAssembly(string filename)
        {
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream(filename);
                var text = string.Empty;

                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    text = reader.ReadToEnd();
                }

                return text;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return string.Empty;
            }
        }
    }
}