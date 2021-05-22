using System.Windows.Input;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalTaskViewModel
    {
        GoalTask[] GoalTasks { get; set; }
        GoalTask SelectedGoalTask { get; set; }
        Goal ParentGoal { get; }

        ICommand DeleteTaskAsyncCommand { get; }
        ICommand SetTaskCompletedAsyncCommand { get; }
        ICommand LoadTasksAsyncCommand { get; }
    }
}