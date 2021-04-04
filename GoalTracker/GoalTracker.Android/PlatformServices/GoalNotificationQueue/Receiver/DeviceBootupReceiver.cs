using Android.App;
using Android.Content;
using Autofac;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services;

namespace GoalTracker.Droid.PlatformServices.GoalNotificationQueue.Receiver
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] {Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted})]
    public class DeviceBootupReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Android.Content.Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionBootCompleted) ||
                intent.Action.Equals(Intent.ActionLockedBootCompleted))
            {
                var container = MainActivity.GetContainer();

                if (container != null)
                {
                    var goalRepository = container.Resolve<IGoalRepository>();
                    var userRepository = container.Resolve<IUserRepository>();
                    var goals = await goalRepository.GetAllAsync();
                    var username = await userRepository.GetUsernameAsync();

                    if (goals != null)
                    {
                        INotificationQueueManager notificationQueueManager = new NotificationQueueManager();

                        var goalCount = 0;
                        foreach (var goal in goals)
                        {
                            var goalRequestCodes = await goalRepository.GetNextRequestCodesForNotificationWithOptions();
                            notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goal,
                                goalRequestCodes, username);
                            goalCount++;
                        }
                    }
                }
            }
        }
    }
}