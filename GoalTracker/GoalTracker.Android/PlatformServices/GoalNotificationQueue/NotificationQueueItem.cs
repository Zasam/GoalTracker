using GoalTracker.Models;
using Android.App;
using GoalTracker.Entities;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue
{
    public class NotificationQueueItem
    {
        public Goal NotificationGoal;
        public PendingIntent PendingNotification { get; set; }

        public NotificationQueueItem(Goal notificationGoal, PendingIntent pendingNotification)
        {
            NotificationGoal = notificationGoal;
            PendingNotification = pendingNotification;
        }
    }
}