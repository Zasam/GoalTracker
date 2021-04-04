using GoalTracker.Services;
using GoalTracker.Entities;

namespace GoalTracker.PlatformServices
{
    public interface INotificationQueueManager
    {
        public bool CancelAlarms(Goal goal);
        public void QueueGoalNotificationBroadcast(IRepository<Goal> repository, Goal goal, int[] requestCodes, string username);
    }
}