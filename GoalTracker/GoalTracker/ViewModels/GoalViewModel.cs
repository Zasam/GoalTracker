using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class GoalViewModel : BaseViewModel, IGoalViewModel
    {
        #region Properties

        #region Repositories

        private readonly IGoalRepository goalRepository;

        public IGoalTaskRepository GoalTaskRepository { get; }
        public IGoalAppointmentRepository GoalAppointmentRepository { get; }

        #endregion // Repositories

        private List<Goal> goals;
        private bool goalHasDueDate;

        public string GoalImage { get; set; }

        public Goal SelectedGoal { get; set; }

        public List<Goal> Goals
        {
            get => goals;
            set
            {
                goals = value;
                OnPropertyChanged();
            }
        }

        public bool GoalHasDueDate
        {
            get => goalHasDueDate;
            set
            {
                goalHasDueDate = value;
                OnPropertyChanged();
            }
        }

        public List<string> GoalNotificationIntervals { get; set; }
        public string GoalTitle { get; set; }
        public string GoalNotes { get; set; }
        public DateTime GoalMinimumStartDate { get; set; }
        public DateTime GoalMinimumEndDate { get; set; }
        public DateTime GoalStartDate { get; set; }
        public DateTime GoalEndDate { get; set; }
        public int GoalNotificationIntervalIndex { get; set; }
        public TimeSpan GoalNotificationTime { get; set; }

        public GoalAppointmentInterval GoalNotificationInterval =>
            (GoalAppointmentInterval) GoalNotificationIntervalIndex;

        #endregion // Properties

        public GoalViewModel(IGoalRepository goalRepository, IGoalAppointmentRepository goalAppointmentRepository,
            IGoalTaskRepository goalTaskRepository)
        {
            this.goalRepository = goalRepository;
            GoalTaskRepository = goalTaskRepository;
            GoalAppointmentRepository = goalAppointmentRepository;

            GoalMinimumStartDate = DateTime.Now.AddHours(1);
            GoalMinimumEndDate = DateTime.Now.AddDays(1);
            GoalStartDate = GoalMinimumStartDate;
            GoalEndDate = GoalMinimumEndDate;
            GoalNotificationIntervalIndex = 3;
            GoalNotificationTime = TimeSpan.FromHours(DateTime.Now.Hour + 1);

            GoalNotificationIntervals = new List<string>();
            foreach (var notificationInterval in Enum.GetValues(typeof(GoalAppointmentInterval)))
                GoalNotificationIntervals.Add(Enum.GetName(typeof(GoalAppointmentInterval), notificationInterval));
        }


        public async Task<Goal> EditGoalAsync(Goal goal, GoalTask[] goalTasks, string username)
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

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();
                //TODO: Alarms canceled properly?
                notificationQueueManager.CancelAlarms(goal);
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goal,
                    new[] {goal.RequestCode, goal.RequestCode - 1, goal.RequestCode - 2},
                    username);
                return goal;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<bool> DeleteGoalAsync(Goal goal)
        {
            try
            {
                await goalRepository.RemoveAsync(goal);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task<GoalTask[]> LoadTasksAsync(Goal parent)
        {
            try
            {
                var goalTasks = await GoalTaskRepository.GetAllByParentAsync(parent);
                return goalTasks.ToArray();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<Goal> AddGoalAsync(Goal goal, GoalTask[] goalTasks, string username)
        {
            try
            {
                var goalNotificationId = await goalRepository.GetNextNotificationId();
                var goalRequestCodes = await goalRepository.GetNextRequestCodesForNotificationWithOptions();
                goal.SetNotificationIndex(goalNotificationId, goalRequestCodes.Max());
                await goalRepository.AddAsync(goal);

                var goalAppointments = goal.GetAppointments();
                await GoalAppointmentRepository.AddRangeAsync(goalAppointments);

                if (goalTasks != null && goalTasks.Any())
                {
                    foreach (var goalTask in goalTasks)
                        await GoalTaskRepository.AddAsync(goalTask);

                    //goalTaskRepository.AddRange(goalTasks);
                    goal.GoalTaskCount = goalTasks.Count();
                }

                await goalRepository.SaveChangesAsync();

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goal, goalRequestCodes,
                    username);

                return goal;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task LoadGoalsAsync()
        {
            try
            {
                var goalCollection = await goalRepository.GetAllAsync();
                Goals = goalCollection.ToList();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}