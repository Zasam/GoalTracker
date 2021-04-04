using Xamarin.UITest;

namespace UnitTest.Xamarin
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            // TODO: Before running tests, build project in release mode, right-click android project, select "archive" and replace the string below, with the new apk destination folder!
            return ConfigureApp.Android.ApkFile(@"C:\Users\Nickl\AppData\Local\Xamarin\Mono for Android\Archives\2021-02-08\GoalTracker.Android 2-08-21 12.26 AM.apkarchive\com.ZasamStudio.GoalTracker.apk").StartApp();
        }
    }
}