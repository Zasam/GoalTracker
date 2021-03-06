using GoalTracker.Services;
using GoalTracker.Entities;

namespace GoalTracker.PlatformServices
{
    public interface INotifier
    {
        public void PushNotification(string title, string message, int notificationId);
        public void PushNotification(IRepository<Goal> repository, string goalTitle, int goalNotificationId, int goalRequestCode, string username);
        public void CancelNotification(int notificationId);
    }
}