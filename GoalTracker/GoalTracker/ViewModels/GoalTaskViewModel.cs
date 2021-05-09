using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class GoalTaskViewModel : BaseViewModel, IGoalTaskViewModel
    {
        #region Repositories

        private readonly IGoalTaskRepository goalTaskRepository;

        #endregion // Repositories

        public Goal ParentGoal { get; }

        #region Properties

        private GoalTask[] goalTasks;

        /// <summary>
        /// All saved task associated with the specified parent
        /// </summary>
        public GoalTask[] GoalTasks
        {
            get => goalTasks;
            set
            {
                goalTasks = value;
                OnPropertyChanged();
            }
        }

        private GoalTask selectedGoalTask;

        /// <summary>
        /// The last loaded / approved task associated with the specified parent
        /// </summary>
        public GoalTask SelectedGoalTask
        {
            get => selectedGoalTask;
            set
            {
                selectedGoalTask = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        /// <summary>
        /// Async command to delete the specified task
        /// </summary>
        public ICommand DeleteTaskAsyncCommand { get; }

        /// <summary>
        /// Async command to set the specified task to completed
        /// </summary>
        public ICommand SetTaskCompletedAsyncCommand { get; }

        /// <summary>
        /// Async command to load all tasks associated with the specified parent
        /// </summary>
        public ICommand LoadTasksAsyncCommand { get; }

        #endregion // Commands

        #endregion // Properties

        // TODO: Only used to check bindings in xaml
        public GoalTaskViewModel()
        {
            throw new InvalidOperationException("Goal task view model shouldn't be initialized through parameterless constructor");
        }

        public GoalTaskViewModel(Goal parent, IGoalTaskRepository goalTaskRepository)
        {
            ParentGoal = parent ?? throw new ArgumentNullException("Passed parent goal is null. Please provide a valid goal to instantiate the goal task viewmodel.");
            this.goalTaskRepository = goalTaskRepository;

            DeleteTaskAsyncCommand = new Command<GoalTask>(async goalTask => await DeleteTaskAsync(goalTask));
            SetTaskCompletedAsyncCommand = new Command<GoalTask>(async goalTask => await SetTaskCompletedAsync(goalTask));
            LoadTasksAsyncCommand = new Command(async () => await LoadTasksAsync());
        }

        private async Task DeleteTaskAsync(GoalTask goalTask)
        {
            try
            {
                await goalTaskRepository.RemoveAsync(goalTask);

                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task SetTaskCompletedAsync(GoalTask goalTask)
        {
            try
            {
                var dbSelectedGoalTask = await goalTaskRepository
                    .GetAllByParentAsync(ParentGoal);
                var sGoalTask = dbSelectedGoalTask.FirstOrDefault(gt => gt.Id == goalTask.Id);

                if (sGoalTask != null)
                    sGoalTask.Completed = !sGoalTask.Completed;

                await goalTaskRepository.SaveChangesAsync();

                SelectedGoalTask = sGoalTask;

                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                var goalTaskCollection = await goalTaskRepository.GetAllByParentAsync(ParentGoal);
                GoalTasks = goalTaskCollection.ToArray();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}