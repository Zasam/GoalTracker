using System;
using Android.Content;
using Autofac;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver
{
    [BroadcastReceiver(Enabled = true)]
    public class GoalFailureUserInputReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Android.Content.Context context, Intent intent)
        {
            try
            {
                if (context == null || intent == null)
                    return;

                var container = MainActivity.GetContainer();
                var goalRepository = container.Resolve<IGoalRepository>();
                var goalAppointmentRepository = container.Resolve<IGoalAppointmentRepository>();

                if (goalRepository == null)
                    return;

                if (intent.Extras == null)
                    return;

                var goalTitle = intent.Extras.GetString("GoalTitle");
                var goalNotificationId = intent.Extras.GetInt("GoalNotificationId");
                var savedGoal = await goalRepository.GetByTitleAsnyc(goalTitle);

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

        private void ValidateAchievements(IAchievementRepository achievementRepository,
            IGoalAppointmentRepository goalDateRepository, Goal savedGoal)
        {
            //TODO: Re-implement validation of achievements!!!
            //var goalAppointments = goalDateRepository.GetAllByGoal(savedGoal);

            //if (savedGoal.ApprovalCount == 10)
            //{
            //    var approval10 = achievementRepository.GetByTitle("GOALAPPROVALGEN10");
            //    if (approval10 != null)
            //    {
            //        approval10.Unlock();
            //        achievementRepository.Commit();
            //        INotifier notifier = new Notifier();
            //        notifier.PushNotification(approval10.Title, approval10.Description, 1400);
            //    }
            //}
            //else if (savedGoal.ApprovalCount == 25)
            //{
            //    var approval25 = achievementRepository.GetByTitle("GOALAPPROVALGEN25");
            //    if (approval25 != null)
            //    {
            //        approval25.Unlock();
            //        achievementRepository.Commit();
            //        INotifier notifier = new Notifier();
            //        notifier.PushNotification(approval25.Title, approval25.Description, 1500);
            //    }
            //}
            //else if (savedGoal.ApprovalCount == 50)
            //{
            //    var approval50 = achievementRepository.GetByTitle("GOALAPPROVALGEN50");
            //    if (approval50 != null)
            //    {
            //        approval50.Unlock();
            //        achievementRepository.Commit();
            //        INotifier notifier = new Notifier();
            //        notifier.PushNotification(approval50.Title, approval50.Description, 1600);
            //    }
            //}
        }
    }
}