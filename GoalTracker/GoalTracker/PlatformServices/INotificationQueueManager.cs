using GoalTracker.Entities;
using GoalTracker.Services.Interface;

namespace GoalTracker.PlatformServices
{
    public interface INotificationQueueManager
    {
        public bool CancelAlarms(Goal goal);
        public void QueueGoalNotificationBroadcast(IGoalRepository repository, Goal goal, int[] requestCodes, string username);
    }
}