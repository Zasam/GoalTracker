using System;
using System.Reflection;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Autofac;
using BQFramework.IO;
using GoalTracker.Context;
using GoalTracker.Droid.PlatformServices.GoalNotificationQueue;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
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
    [Activity(
        Label = "GoalTracker",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode
                               | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : FormsAppCompatActivity
    {
        public static readonly string ChannelId = "goaltracker_notifications";

        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            $"{nameof(GoalTracker)}.db");

        private static IContainer container;

        private NotificationManager NotificationManager =>
            (NotificationManager) GetSystemService(NotificationService);

        public static IContainer GetContainer()
        {
            try
            {
                if (container != null) return container;

                DependencyRegistration();
                return container;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(savedInstanceState);

                var assembly = typeof(MainActivity).GetTypeInfo().Assembly;

                SyncfusionLicenseProvider.RegisterLicense(
                    FileHelper.GetTextFromFile("GoalTracker.Droid.SyncfusionLicense.txt", assembly));

                AppCenter.Start(
                    FileHelper.GetTextFromFile("GoalTracker.Droid.AppCenterSecrets.txt", assembly),
                    typeof(Analytics), typeof(Crashes));

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
                DependencyRegistration();

                LoadApplication(new App(container));
            }
            catch (Exception)
            {
                //TODO: App center isn't initialized here!
                //Crashes.TrackError(ex, LoggingHelpers.GetCrashProperties(nameof(MainActivity), nameof(OnCreate)));
            }
        }

        private static void DependencyRegistration()
        {
            // Create builder and default context to use
            var builder = new ContainerBuilder();

            // Register the custom database context
            builder.RegisterType<GoalTrackerContext>().As<IGoalTrackerContext>().SingleInstance();

            // Register the generic repository *NOT NEEDED: IMPLEMENTED SPECIFIC REPOSITORIES*
            // builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            // Register all repositories
            builder.RegisterType<GoalRepository>().As<IGoalRepository>().SingleInstance();
            builder.RegisterType<AchievementRepository>().As<IAchievementRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<GoalAppointmentRepository>().As<IGoalAppointmentRepository>().SingleInstance();
            builder.RegisterType<GoalTaskRepository>().As<IGoalTaskRepository>().SingleInstance();

            // Register all viewmodels
            builder.RegisterType<GoalViewModel>().As<IGoalViewModel>().SingleInstance();
            builder.RegisterType<CalendarViewModel>().As<ICalendarViewModel>().SingleInstance();
            builder.RegisterType<SettingsViewModel>().As<ISettingsViewModel>().SingleInstance();
            builder.RegisterType<RegistrationViewModel>().As<IRegistrationViewModel>().SingleInstance();

            container = builder.Build();
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