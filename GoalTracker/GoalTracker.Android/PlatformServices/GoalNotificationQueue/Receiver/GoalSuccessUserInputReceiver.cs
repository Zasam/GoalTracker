using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Autofac;
using GoalTracker.DI;
using GoalTracker.Droid.PlatformServices.Notification;
using GoalTracker.Entities;
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

                var container = Bootstrapper.GetContainer();
                var goalRepository = container.Resolve<IGoalRepository>();
                var goalAppointmentRepository = container.Resolve<IGoalAppointmentRepository>();
                var achievementRepository = container.Resolve<IAchievementRepository>();

                if (goalRepository == null)
                    return;

                if (achievementRepository == null)
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

                var goalAppointment = await goalAppointmentRepository.GetByParentAndAppointmentDateAsync(savedGoal, DateTime.Now);
                if (goalAppointment != null)
                {
                    goalAppointment.Approve(true);
                    await goalAppointmentRepository.SaveChangesAsync();
                }

                var goalAppointments = await goalAppointmentRepository.GetAllByParentAsync(savedGoal);
                await CheckForUnlockedAchievements(notifier, achievementRepository, goalAppointments);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private static async Task CheckForUnlockedAchievements(INotifier notifier, IAchievementRepository achievementRepository, IEnumerable<GoalAppointment> goalAppointments)
        {
            var appointments = goalAppointments.ToArray();
            var approvedAppointments = appointments.Select(a => a.Approved).Count();
            var successfulApprovedAppointments = appointments.Select(a => a.Approved && a.Success.HasValue && a.Success.Value).Count();

            Achievement unlockedAchievement = null;

            switch (approvedAppointments)
            {
                case 10:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALAPPROVAL10");
                    break;
                case 25:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALAPPROVAL25");
                    break;
                case 50:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALAPPROVAL50");
                    break;
            }

            if (unlockedAchievement != null)
            {
                unlockedAchievement.Unlock();
                notifier.PushNotification("Neuer Erfolg freigeschaltet", "Erfolg freigeschaltet: " + unlockedAchievement.Title + Environment.NewLine + unlockedAchievement.Description, 999);
                unlockedAchievement = null;
            }

            switch (successfulApprovedAppointments)
            {
                case 10:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALSUCCESSAPPROVAL10");
                    break;
                case 25:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALSUCCESSAPPROVAL25");
                    break;
                case 50:
                    unlockedAchievement = await achievementRepository.GetByInternalTag("GOALSUCCESSAPPROVAL50");
                    break;
            }

            if (unlockedAchievement != null)
            {
                unlockedAchievement.Unlock();
                notifier.PushNotification("Neuer Erfolg freigeschaltet", "Erfolg freigeschaltet: " + unlockedAchievement.Title + Environment.NewLine + unlockedAchievement.Description, 999);
            }
        }
    }
}