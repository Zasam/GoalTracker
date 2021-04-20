using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue
{
    public class NotificationQueueManager : INotificationQueueManager
    {
        public NotificationQueueManager()
        {
            NotificationQueue ??= new List<NotificationQueueItem>();
        }

        private List<NotificationQueueItem> NotificationQueue { get; }

        public void QueueGoalNotificationBroadcast(IGoalRepository repository, Goal goal, int[] requestCodes,
            string username)
        {
            try
            {
                // Calculate notification datetime in milliseconds since 1970
                var notificationDateTime = new DateTime(goal.StartDate.Year, goal.StartDate.Month, goal.StartDate.Day,
                    goal.NotificationTime.Hours, goal.NotificationTime.Minutes, 00);
                var notificationDateTimeInMilliseconds = notificationDateTime.GetMillisecondsSince1970();

                // Convert supplied goal to json
                var goalJson = JsonConvert.SerializeObject(goal, Formatting.None,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

                // Create a new intent and pending intent, which should be used to pend a new alarm for the notification
                var notificationAlarmIntent = new Intent(Application.Context, typeof(GoalNotificationReceiver));
                notificationAlarmIntent.AddFlags(ActivityFlags.ReceiverForeground);
                notificationAlarmIntent.PutExtra("Goal", goalJson);
                notificationAlarmIntent.PutExtra("Username", username);

                // Create a pending intent as broadcast
                var pendingNotificationAlarmIntent = PendingIntent.GetBroadcast(Application.Context, goal.RequestCode, notificationAlarmIntent, PendingIntentFlags.UpdateCurrent); //TODO: Does this need Flag CancelCurrent instead of UpdateCurrent?

                // Create a new notificationQueueItem and add it to the notification queue, to cancel pending intents later
                NotificationQueue.Add(new NotificationQueueItem(goal, pendingNotificationAlarmIntent));

                // Get the alarm manager instance from the supplied context
                var alarmManager =
                    (AlarmManager) Application.Context.GetSystemService(Android.Content.Context.AlarmService);
                alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, notificationDateTimeInMilliseconds,
                    pendingNotificationAlarmIntent);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public bool CancelAlarms(Goal goal)
        {
            try
            {
                var notificationQueueItem = NotificationQueue?.FirstOrDefault(nqi => nqi.NotificationGoal == goal);
                if (notificationQueueItem == null) return false;
                var alarmManager = (AlarmManager) Application.Context.GetSystemService(Android.Content.Context.AlarmService);
                alarmManager?.Cancel(notificationQueueItem.PendingNotification);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }
    }
}