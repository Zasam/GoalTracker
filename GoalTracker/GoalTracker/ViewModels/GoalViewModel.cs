using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class GoalViewModel : BaseViewModel, IGoalViewModel
    {
        #region Repositories

        public IGoalTaskRepository GoalTaskRepository { get; }
        public IGoalAppointmentRepository GoalAppointmentRepository { get; }

        private readonly IGoalRepository goalRepository;
        private readonly IUserRepository userRepository;

        #endregion // Repositories

        #region Properties

        private string goalTitle;

        /// <summary>
        /// Title of the goal that is added or should be created
        /// </summary>
        public string GoalTitle
        {
            get => goalTitle;
            set
            {
                goalTitle = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        private string goalNotes;

        /// <summary>
        /// Notes of the goal that is added or should be created
        /// </summary>
        public string GoalNotes
        {
            get => goalNotes;
            set
            {
                goalNotes = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        /// <summary>
        /// Minimum start date of the goal that is added or should be created
        /// </summary>
        public DateTime GoalMinimumStartDate { get; set; }

        /// <summary>
        /// Minimum end date of the goal that is added or should be created
        /// </summary>
        public DateTime GoalMinimumEndDate { get; set; }

        private DateTime goalStartDate;

        /// <summary>
        /// Start date of the goal that is added or should be created
        /// </summary>
        public DateTime GoalStartDate
        {
            get => goalStartDate;
            set
            {
                goalStartDate = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        private DateTime goalEndDate;

        /// <summary>
        /// End date of the goal that is added or should be created
        /// </summary>
        public DateTime GoalEndDate
        {
            get => goalEndDate;
            set
            {
                goalEndDate = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        private TimeSpan goalNotificationTime;

        /// <summary>
        /// Notification time of the goal that is added or should be created
        /// </summary>
        public TimeSpan GoalNotificationTime
        {
            get => goalNotificationTime;
            set
            {
                goalNotificationTime = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        private int goalNotificationIntervalIndex;

        /// <summary>
        /// Notification interval index of the goal that is added or should be created
        /// </summary>
        public int GoalNotificationIntervalIndex
        {
            get => goalNotificationIntervalIndex;
            set
            {
                goalNotificationIntervalIndex = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        /// <summary>
        /// Notification interval of the goal that is added or should be created
        /// </summary>
        public GoalAppointmentInterval GoalNotificationInterval =>
            (GoalAppointmentInterval) GoalNotificationIntervalIndex;

        private string goalImage;

        /// <summary>
        /// Image of the goal that is added or should be created
        /// </summary>
        public string GoalImage
        {
            get => goalImage;
            set
            {
                goalImage = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        private bool goalHasDueDate;

        /// <summary>
        /// Indication if a due date is specified of the goal that is added or should be created
        /// </summary>
        public bool GoalHasDueDate
        {
            get => goalHasDueDate;
            set
            {
                goalHasDueDate = value;
                OnPropertyChanged();
                BindedGoal = new Goal(GoalTitle, GoalNotes, GoalStartDate, GoalHasDueDate, GoalEndDate, GoalNotificationInterval, GoalNotificationTime, GoalImage);
            }
        }

        /// <summary>
        /// The currently selected goal
        /// </summary>
        public Goal SelectedGoal { get; set; }

        private List<Goal> goals;

        /// <summary>
        /// Collection of all loaded goals
        /// </summary>
        public List<Goal> Goals
        {
            get => goals;
            set
            {
                goals = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enumeration of possible notification intervals
        /// </summary>
        public List<string> GoalNotificationIntervals { get; }

        private List<string> goalTaskTitles;

        public List<string> GoalTaskTitles
        {
            get => goalTaskTitles;
            set
            {
                goalTaskTitles = value;
                OnPropertyChanged();
            }
        }

        private List<GoalTask> goalTasks;

        /// <summary>
        /// Collection of all loaded tasks associated with the specified parent
        /// </summary>
        public List<GoalTask> GoalTasks
        {
            get => goalTasks;
            set
            {
                goalTasks = value;
                OnPropertyChanged();
            }
        }

        private bool isRefreshing;

        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private ISettingViewModel settingViewModel;

        public ISettingViewModel SettingViewModel
        {
            get => settingViewModel;
            set
            {
                settingViewModel = value;
                OnPropertyChanged();
            }
        }

        private Goal bindedGoal;

        public Goal BindedGoal
        {
            get => bindedGoal;
            private set
            {
                bindedGoal = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        /// <summary>
        /// Async command to load all goals into the collection
        /// </summary>
        public ICommand LoadGoalsAsyncCommand { get; }

        /// <summary>
        /// Async command to add a new goal with the binded values
        /// </summary>
        public ICommand AddGoalAsyncCommand { get; }

        /// <summary>
        /// Async command to edit the specified goal with the binded values
        /// </summary>
        public ICommand EditGoalAsyncCommand { get; }

        /// <summary>
        /// Async command to delete the specified goal
        /// </summary>
        public ICommand DeleteGoalAsyncCommand { get; }

        /// <summary>
        /// Async command to load all tasks into the collection associated with the parent
        /// </summary>
        public ICommand LoadTasksAsyncCommand { get; }

        #endregion // Commands

        #endregion // Properties

        // TODO: Only used to check bindings in xaml
        public GoalViewModel()
        {
            throw new InvalidOperationException("Goal view model shouldn't be initialized through parameterless constructor");
        }

        public GoalViewModel(IGoalRepository goalRepository, IGoalAppointmentRepository goalAppointmentRepository, IGoalTaskRepository goalTaskRepository, IUserRepository userRepository, ISettingViewModel settingViewModel)
        {
            this.goalRepository = goalRepository;
            this.userRepository = userRepository;

            GoalTaskRepository = goalTaskRepository;
            GoalAppointmentRepository = goalAppointmentRepository;
            SettingViewModel = settingViewModel;

            GoalMinimumStartDate = DateTime.Now;
            GoalStartDate = DateTime.Now.AddHours(1);
            GoalMinimumEndDate = DateTime.Now.AddDays(1);
            GoalEndDate = DateTime.Now.AddDays(1);
            GoalNotificationIntervalIndex = 3;
            GoalNotificationTime = TimeSpan.FromHours(DateTime.Now.Hour + 1);

            GoalNotificationIntervals = new List<string>();
            foreach (var notificationInterval in Enum.GetValues(typeof(GoalAppointmentInterval)))
                GoalNotificationIntervals.Add(Enum.GetName(typeof(GoalAppointmentInterval), notificationInterval));

            LoadGoalsAsyncCommand = new Command(async () => await LoadGoalsAsync());
            AddGoalAsyncCommand = new Command<Goal>(async goal => await AddGoalAsync(goal));
            EditGoalAsyncCommand = new Command<Goal>(async goal => await EditGoalAsync(goal));
            DeleteGoalAsyncCommand = new Command<Goal>(async goal => await DeleteGoalAsync(goal));
            LoadTasksAsyncCommand = new Command<Goal>(async goal => await LoadTasksAsync(goal));
        }

        private async Task EditGoalAsync(Goal goal)
        {
            try
            {
                goal.Title = GoalTitle;
                goal.Notes = GoalNotes;
                goal.StartDate = GoalStartDate;
                goal.HasDueDate = GoalHasDueDate;
                goal.EndDate = GoalEndDate;
                goal.GoalAppointmentInterval = GoalNotificationInterval;
                goal.NotificationTime = GoalNotificationTime;
                goal.DetailImage = GoalImage;

                await goalRepository.SaveChangesAsync();

                var currentGoalTasks = await GoalTaskRepository.GetAllByParentAsync(goal);
                await GoalTaskRepository.RemoveRangeAsync(currentGoalTasks);

                if (goalTasks.Any())
                {
                    foreach (var goalTask in goalTasks)
                        await GoalTaskRepository.AddAsync(goalTask);

                    goal.GoalTaskCount = goalTasks.Count();
                }

                await GoalTaskRepository.SaveChangesAsync();

                var user = await userRepository.GetUserAsync();

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();
                //TODO: Alarms canceled properly?
                notificationQueueManager.CancelAlarms(goal);
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goal,
                    new[] {goal.RequestCode, goal.RequestCode - 1, goal.RequestCode - 2},
                    user.Name);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task DeleteGoalAsync(Goal goal)
        {
            try
            {
                await goalRepository.RemoveAsync(goal);

                await LoadGoalsAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadTasksAsync(Goal parent)
        {
            try
            {
                var loadedGoalTasks = await GoalTaskRepository.GetAllByParentAsync(parent);
                GoalTasks = loadedGoalTasks.ToList();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task AddGoalAsync(Goal goal)
        {
            try
            {
                var goalNotificationId = await goalRepository.GetNextNotificationId();
                var goalRequestCodes = await goalRepository.GetNextRequestCodesForNotificationWithOptions();
                goal.SetNotificationIndex(goalNotificationId, goalRequestCodes.Max());
                await goalRepository.AddAsync(goal);

                var goalAppointments = goal.GetAppointments();
                await GoalAppointmentRepository.AddRangeAsync(goalAppointments);

                var tasks = new List<GoalTask>();
                foreach (var goalTaskTitle in GoalTaskTitles)
                {
                    var task = new GoalTask(goal, goalTaskTitle, string.Empty, false);
                    tasks.Add(task);
                }

                if (tasks.Any())
                    await GoalTaskRepository.AddRangeAsync(tasks);

                goal.GoalTaskCount = tasks.Count;
                await goalRepository.SaveChangesAsync();

                var user = await userRepository.GetUserAsync();

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goal, goalRequestCodes,
                    user.Name);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadGoalsAsync()
        {
            try
            {
                IsRefreshing = true;

                var goalCollection = await goalRepository.GetAllAsync();
                Goals = goalCollection.ToList();

                //TODO: Any option to move this to xaml?
                if (settingViewModel.LoadUserAsyncCommand.CanExecute(null))
                    settingViewModel.LoadUserAsyncCommand.Execute(null);

                if (settingViewModel.LoadAchievementsAsyncCommand.CanExecute(null))
                    settingViewModel.LoadAchievementsAsyncCommand.Execute(null);

                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}