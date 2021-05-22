using System;
using System.Reflection;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using BQFramework.IO;
using GoalTracker.DI;
using GoalTracker.Droid.PlatformServices.GoalNotificationQueue;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.PlatformServices;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Syncfusion.Licensing;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using Environment = System.Environment;
using Messenger = GoalTracker.Droid.PlatformServices.Messaging.Messenger;
using Path = System.IO.Path;
using Platform = Xamarin.Essentials.Platform;

namespace GoalTracker.Droid
{
    [Activity(Label = "GoalTracker", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : FormsAppCompatActivity
    {
        public static readonly string ChannelId = "goaltracker_notifications";

        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            $"{nameof(GoalTracker)}.db");

        private NotificationManager NotificationManager =>
            (NotificationManager) GetSystemService(NotificationService);

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            // First try block, where exception is just thrown, later on the app center is used to track errors, after it is correctly initialized
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(savedInstanceState);

                var assembly = typeof(MainActivity).GetTypeInfo().Assembly;

                var syncfusionLicense =
                    FileHelper.GetTextFromFile("GoalTracker.Droid.SyncfusionLicense.txt", assembly);
                var appCenterSecrets =
                    FileHelper.GetTextFromFile("GoalTracker.Droid.AppCenterSecrets.txt", assembly);

                if (!string.IsNullOrWhiteSpace(syncfusionLicense) && !string.IsNullOrWhiteSpace(appCenterSecrets))
                {
                    SyncfusionLicenseProvider.RegisterLicense(syncfusionLicense);
                    AppCenter.Start(appCenterSecrets,
                        typeof(Analytics), typeof(Crashes));
                }
                else
                {
                    //TODO: Other method to inform the user than through toast message? This needs to be tested!
                    var messenger = new Messenger();
                    messenger.LongMessage(
                        "One or more external services weren't correctly authorized. Purchase a licensed copy of this app to get all features.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                Window?.SetNavigationBarColor(Color.Rgb(196, 196, 196));
                Window?.SetStatusBarColor(Color.Rgb(196, 196, 196));

                Platform.Init(this, savedInstanceState);
                Forms.Init(this, savedInstanceState);

                // Create all notification channels for all different types of push notifications
                CreateNotificationChannel("GoalTracker Notifications", "Get notifications to track your goals");

                // Register all platform specific services with xamarin forms.
                DependencyService.Register<IMessenger, Messenger>();
                DependencyService.Register<INotifier, Notifier>();
                DependencyService.Register<INotificationQueueManager, NotificationQueueManager>();

                // Register all needed services by autofac
                var app = await Bootstrapper.Run();

                LoadApplication(app);
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void CreateNotificationChannel(string channelName, string channelDescription)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)

                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;

            var channel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Max)
            {
                Description = channelDescription
            };

            NotificationManager.CreateNotificationChannel(channel);
        }
    }
}