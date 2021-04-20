using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalTaskViewModel : BaseViewModel, IGoalTaskViewModel
    {
        private readonly IGoalTaskRepository goalTaskRepository;

        private GoalTask goalTask;
        private GoalTask[] goalTasks;
        private readonly Goal parent;

        #region Properties

        #region ReadOnly Bindings

        public GoalTask[] GoalTasks
        {
            get => goalTasks;
            private set
            {
                goalTasks = value;
                OnPropertyChanged();
            }
        }

        public GoalTask SelectedGoalTask
        {
            get => goalTask;
            private set
            {
                goalTask = value;
                OnPropertyChanged();
            }
        }

        public string ParentTitle => parent == null ? "N/A" : parent.Title;

        #endregion // ReadOnly Bindings

        #endregion // Properties

        public GoalTaskViewModel(Goal parent, IGoalTaskRepository goalTaskRepository)
        {
            this.goalTaskRepository = goalTaskRepository;
            this.parent = parent;
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
                    .GetAllByParentAsync(parent);
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
                var goalTaskCollection = await goalTaskRepository.GetAllByParentAsync(parent);
                GoalTasks = goalTaskCollection.ToArray();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void SetTask(GoalTask task)
        {
            SelectedGoalTask = task;
        }
    }
}