using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
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

        public void QueueGoalNotificationBroadcast(Goal goal, int[] requestCodes, string username, bool bootup)
        {
            try
            {
                DateTime notificationDateTime;

                if (bootup)
                {
                    //TODO: Test in depth if this is working
                    // Get the next appointment which is not approved and has the nearest appointment date in the feature (relative to the current datetime)
                    var nextUnapprovedAppointmentInFuture = goal.GoalAppointments.Aggregate((currentNearestAppointment, nextNearestAppointment) =>
                        currentNearestAppointment.AppointmentDate <= DateTime.Now || currentNearestAppointment.Approved ? nextNearestAppointment : currentNearestAppointment);
                    notificationDateTime = nextUnapprovedAppointmentInFuture.AppointmentDate;
                }
                else
                {
                    notificationDateTime = new DateTime(goal.StartDate.Year, goal.StartDate.Month, goal.StartDate.Day, goal.NotificationTime.Hours, goal.NotificationTime.Minutes, 00);
                }

                var notificationDateTimeInMilliseconds = notificationDateTime.GetMillisecondsSince1970();
                var goalJson = JsonConvert.SerializeObject(goal, Formatting.None, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

                // Create a new intent and pending intent, which should be used to pend a new alarm for the notification
                var notificationAlarmIntent = new Intent(Application.Context, typeof(GoalNotificationReceiver));
                notificationAlarmIntent.AddFlags(ActivityFlags.ReceiverForeground);
                notificationAlarmIntent.PutExtra("Goal", goalJson);
                notificationAlarmIntent.PutExtra("Username", username);
                var pendingNotificationAlarmIntent = PendingIntent.GetBroadcast(Application.Context, goal.RequestCode, notificationAlarmIntent, PendingIntentFlags.UpdateCurrent); //TODO: Does this need Flag CancelCurrent instead of UpdateCurrent?

                // Create a new notificationQueueItem and add it to the notification queue, to cancel pending intents later
                NotificationQueue.Add(new NotificationQueueItem(goal, pendingNotificationAlarmIntent));

                var alarmManager = (AlarmManager) Application.Context.GetSystemService(Android.Content.Context.AlarmService);
                alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, notificationDateTimeInMilliseconds, pendingNotificationAlarmIntent);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void CancelAlarms(Goal goal)
        {
            try
            {
                var notificationQueueItem = NotificationQueue?.FirstOrDefault(nqi => nqi.NotificationGoal == goal);
                if (notificationQueueItem == null) return;
                var alarmManager = (AlarmManager) Application.Context.GetSystemService(Android.Content.Context.AlarmService);
                alarmManager?.Cancel(notificationQueueItem.PendingNotification);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}