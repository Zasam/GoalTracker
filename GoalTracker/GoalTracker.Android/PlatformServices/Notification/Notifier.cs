using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.Core.App;
using GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.PlatformServices;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(Notifier))]

namespace GoalTracker.Droid.PlatformServices.Notification
{
    public class Notifier : INotifier
    {
        public void PushNotification(string goalTitle, int goalNotificationId,
            int goalRequestCode, string username)
        {
            try
            {
                // Get the current notification manager from the application context
                var notificationManager = NotificationManagerCompat.From(Application.Context);

                // Create the success notification option for the notification
                var successIntent = new Intent(Application.Context, typeof(GoalSuccessUserInputReceiver));

                // Pass the needed values to the input receiver, so needed actions can be performed
                successIntent.PutExtra("GoalTitle", goalTitle);
                successIntent.PutExtra("GoalNotificationId", goalNotificationId);

                // Get the pending intent as broadcast
                var successPendingIntent = PendingIntent.GetBroadcast(Application.Context, goalRequestCode - 1, successIntent, PendingIntentFlags.UpdateCurrent);

                // Create the failure notification option for the notification
                var failureIntent = new Intent(Application.Context, typeof(GoalFailureUserInputReceiver));

                // Pass the needed values to the input receiver, so needed actions can be performed
                failureIntent.PutExtra("GoalTitle", goalTitle);
                failureIntent.PutExtra("GoalNotificationId", goalNotificationId);

                // Get the pending intent as broadcast
                var failurePendingIntent = PendingIntent.GetBroadcast(Application.Context, goalRequestCode - 2,
                    failureIntent, PendingIntentFlags.UpdateCurrent);

                // Large icon resource bitmap conversion
                var imagefileName = "Icon.png";
                // Remove the file extension from the image filename
                imagefileName = imagefileName.Replace(".jpg", "").Replace(".png", "");

                // Retrieving the local Resource ID from the name
                var id = (int) typeof(Resource.Drawable).GetField(imagefileName).GetValue(null);

                // Converting Drawable Resource to Bitmap
                var largeIcon = BitmapFactory.DecodeResource(Application.Context.Resources, id);

                // Instantiate the builder and set notification elements
                var builder = new NotificationCompat.Builder(Application.Context, MainActivity.ChannelId)
                    .SetSmallIcon(Resource.Drawable.Icon)
                    .SetContentTitle("Erinnerung von GoalTracker: " + goalTitle)
                    //.SetContentText("TEXT") | ContentText is replaced by BigTextStyle 
                    .SetLargeIcon(largeIcon)
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText("Hey " + username +
                                                                            " Neues von GoalTracker" +
                                                                            Environment.NewLine +
                                                                            "Erinnerung an dein Ziel: " + goalTitle))
                    .AddAction(Resource.Drawable.Success, "Erfolgreich", successPendingIntent)
                    .AddAction(Resource.Drawable.Failed, "Noch ein Versuch", failurePendingIntent)
                    .SetPriority(NotificationCompat.PriorityHigh)
                    .SetVisibility(NotificationCompat.VisibilityPublic);

                // Build the notification
                notificationManager.Notify(goalNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void PushNotification(string title, string message, int notificationId)
        {
            try
            {
                // Instantiate the builder and set notification elements
                var builder = new NotificationCompat.Builder(Application.Context, MainActivity.ChannelId)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetSmallIcon(Resource.Drawable.Icon)
                    .SetPriority(NotificationCompat.PriorityHigh)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetAutoCancel(true);

                // Build the notification
                var notificationManager = NotificationManagerCompat.From(Application.Context);
                notificationManager.Notify(notificationId, builder.Build());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void CancelNotification(int notificationId)
        {
            try
            {
                var notificationManager = NotificationManagerCompat.From(Application.Context);
                notificationManager.Cancel(notificationId);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}