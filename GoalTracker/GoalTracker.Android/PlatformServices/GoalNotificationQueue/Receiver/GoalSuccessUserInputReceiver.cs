using System;
using Android.Content;
using Autofac;
using GoalTracker.DI;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.PlatformServices;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver
{
    [BroadcastReceiver(Enabled = true)]
    public class GoalSuccessUserInputReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Android.Content.Context context, Intent intent)
        {
            try
            {
                if (context == null || intent == null)
                    return;

                var container = await Bootstrapper.GetContainer();
                var goalRepository = container.Resolve<IGoalRepository>();
                var goalAppointmentRepository = container.Resolve<IGoalAppointmentRepository>();

                if (goalRepository == null)
                    return;

                if (intent.Extras == null)
                    return;

                var goalTitle = intent.Extras.GetString("GoalTitle");
                var goalNotificationId = intent.Extras.GetInt("GoalNotificationId");
                var savedGoal = await goalRepository.GetByTitleAsync(goalTitle);

                INotifier notifier = new Notifier();
                notifier.CancelNotification(goalNotificationId);

                if (savedGoal == null)
                    return;

                //TODO: Really get only one appointment with this method? Inspect further!!!
                var goalAppointment = await goalAppointmentRepository.GetByParentAndDayAsync(savedGoal, DateTime.Now);
                goalAppointment.Approved = true;
                goalAppointment.Success = false;
                await goalAppointmentRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}