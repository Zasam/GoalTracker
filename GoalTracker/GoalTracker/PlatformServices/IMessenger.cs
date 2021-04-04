namespace GoalTracker.PlatformServices
{
    public interface IMessenger
    {
        void LongMessage(string message);
        void ShortMessage(string message);
    }
}