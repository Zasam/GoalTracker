using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalTaskViewModel
    {
        string ParentTitle { get; }
        GoalTask[] GoalTasks { get; }
        GoalTask SelectedGoalTask { get; }
        Task LoadTasksAsync();
        Task<bool> DeleteTaskAsync(GoalTask goalTask);
        Task<bool> SetTaskCompletedAsync();
        void SetTask(GoalTask task);
    }
}