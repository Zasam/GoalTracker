using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels
{
    public interface IGoalTaskViewModel
    {
        public Goal Parent { get; set; }
        public string ParentTitle { get; }
        public GoalTask[] GoalTasks { get; set; }
        public GoalTask SelectedGoalTask { get; set; }
        public Task LoadTasks();
    }
}