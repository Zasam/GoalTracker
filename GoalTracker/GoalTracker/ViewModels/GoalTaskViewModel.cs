using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalTaskViewModel : BaseViewModel, IGoalTaskViewModel
    {
        #region Properties

        #region Repositories

        private readonly IGoalTaskRepository goalTaskRepository;

        #endregion // Repositories

        private GoalTask goalTask;
        private GoalTask[] goalTasks;

        public GoalTask[] GoalTasks
        {
            get => goalTasks;
            set
            {
                goalTasks = value;
                OnPropertyChanged();
            }
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

        #endregion // Properties

        public GoalTaskViewModel(Goal parent, IGoalTaskRepository goalTaskRepository)
        {
            this.goalTaskRepository = goalTaskRepository;
            Parent = parent;
        }

        public async Task<bool> DeleteTaskAsync(GoalTask goalTask)
        {
            try
            {
                await goalTaskRepository.RemoveAsync(goalTask);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task<bool> SetTaskCompletedAsync()
        {
            try
            {
                var dbSelectedGoalTask = await goalTaskRepository
                    .GetAllByParentAsync(Parent);
                var selectedGoalTask = dbSelectedGoalTask.FirstOrDefault(gt => gt.Id == SelectedGoalTask.Id);

                if (selectedGoalTask != null)
                    selectedGoalTask.Completed = !selectedGoalTask.Completed;
                await goalTaskRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task LoadTasksAsync()
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