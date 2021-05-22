using GoalTracker.Entities;

namespace GoalTracker.PlatformServices
{
    public interface INotificationQueueManager
    {
        public void CancelAlarms(Goal goal);
        public void QueueGoalNotificationBroadcast(Goal goal, int[] requestCodes, string username, bool bootup);
    }
}