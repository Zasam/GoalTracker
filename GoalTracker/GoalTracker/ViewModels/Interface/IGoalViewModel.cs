using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalViewModel
    {
        public IGoalTaskRepository GoalTaskRepository { get; }
        public IGoalAppointmentRepository GoalAppointmentRepository { get; }
        public List<Goal> Goals { get; set; }
        public List<string> GoalNotificationIntervals { get; set; }
        public string GoalImage { get; set; }
        public Goal SelectedGoal { get; set; }
        public string GoalTitle { get; set; }
        public string GoalNotes { get; set; }
        public DateTime GoalMinimumStartDate { get; set; }
        public DateTime GoalMinimumEndDate { get; set; }
        public DateTime GoalStartDate { get; set; }
        public bool GoalHasDueDate { get; set; }
        public DateTime GoalEndDate { get; set; }
        public int GoalNotificationIntervalIndex { get; set; }
        public GoalAppointmentInterval GoalNotificationInterval { get; }
        public TimeSpan GoalNotificationTime { get; set; }
        Task LoadGoalsAsync();
        Task<Goal> AddGoalAsync(Goal goal, GoalTask[] goalTasks, string username);
        Task<Goal> EditGoalAsync(Goal goal, GoalTask[] goalTasks, string username);
        Task<GoalTask[]> LoadTasksAsync(Goal parent);
        Task<bool> DeleteGoalAsync(Goal goal);
    }
}