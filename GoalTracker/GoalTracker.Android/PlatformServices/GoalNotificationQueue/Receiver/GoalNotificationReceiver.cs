using System;
using Android.App;
using Android.Content;
using Autofac;
using GoalTracker.DI;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver
{
    [BroadcastReceiver(Enabled = true)]
    public class GoalNotificationReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Android.Content.Context context, Intent intent)
        {
            INotifier notifier = new Notifier();

            try
            {
                // Check if the intent is valid and all extras has been supplied
                if (intent?.Extras != null)
                {
                    // Get the goal model json from the supplied extras
                    var goalJson = intent.Extras.GetString("Goal");
                    var username = intent.Extras.GetString("Username");

                    // Check if a valid goal model has been passed by json
                    if (!string.IsNullOrWhiteSpace(goalJson))
                    {
                        // Create a new intent to repeat the alarm for the received notification
                        var intentForRepeat = new Intent(context, typeof(GoalNotificationReceiver));
                        intentForRepeat.AddFlags(ActivityFlags.ReceiverForeground);

                        // Supply the model which has been received, so the next alarm works as well
                        intentForRepeat.PutExtra("Goal", goalJson);
                        intentForRepeat.PutExtra("Username", username);

                        // Deserialize the supplied model
                        var notificationGoal = JsonConvert.DeserializeObject<Goal>(goalJson);

                        // Calculate the datetime (in ms) when the next notification alarm should be triggered / received
                        var goalNotificationIntervalInMs = notificationGoal.GetGoalIntervalInMilliseconds();
                        var notificationDateTimeInMilliseconds = DateTime.Now.GetMillisecondsSince1970() + goalNotificationIntervalInMs;

                        // Create a pending intent as broadcast
                        var pendingNotificationIntent = PendingIntent.GetBroadcast(context, notificationGoal.RequestCode, intentForRepeat, PendingIntentFlags.UpdateCurrent); // TODO: Does this need Flag CancelCurrent?!?

                        // Re-queue alarm for the goal notification at the specified datetime (in ms) and with the created pending intent
                        var alarmManager = (AlarmManager) Application.Context.GetSystemService(Android.Content.Context.AlarmService);
                        alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, notificationDateTimeInMilliseconds, pendingNotificationIntent);

                        var container = await Bootstrapper.GetContainer();
                        var repository = container.Resolve<IGoalRepository>();

                        // Push the notification which was received by the alarm
                        notifier.PushNotification(repository, notificationGoal.Title, notificationGoal.NotificationId, notificationGoal.RequestCode, username);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}