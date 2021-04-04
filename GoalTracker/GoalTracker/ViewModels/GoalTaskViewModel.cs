using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalTaskViewModel : BaseViewModel, IGoalTaskViewModel
    {
        private readonly IGoalTaskRepository goalTaskRepository;
        private GoalTask goalTask;
        private GoalTask[] goalTasks;

        public GoalTaskViewModel(Goal parent, IGoalTaskRepository goalTaskRepository)
        {
            this.goalTaskRepository = goalTaskRepository;
            Parent = parent;
        }

        public GoalTask SelectedGoalTask
        {
            get => goalTask;
            set
            {
                goalTask = value;
                OnPropertyChanged();
            }
        }

        public Goal Parent { get; set; }

        public string ParentTitle => Parent == null ? "N/A" : Parent.Title;

        public GoalTask[] GoalTasks
        {
            get => goalTasks;
            set
            {
                goalTasks = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadTasks()
        {
            try
            {
                var goalTaskCollection = await goalTaskRepository.GetAllByParentAsync(Parent);
                GoalTasks = goalTaskCollection.ToArray();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}