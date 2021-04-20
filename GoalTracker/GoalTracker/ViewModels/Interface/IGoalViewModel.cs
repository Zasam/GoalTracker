using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalViewModel
    {
        IGoalTaskRepository GoalTaskRepository { get; }
        IGoalAppointmentRepository GoalAppointmentRepository { get; }

        List<Goal> Goals { get; }
        Goal SelectedGoal { get; }
        List<string> GoalNotificationIntervals { get; }

        string GoalTitle { get; set; }
        string GoalNotes { get; set; }
        DateTime GoalMinimumStartDate { get; set; }
        DateTime GoalMinimumEndDate { get; set; }
        DateTime GoalStartDate { get; set; }
        bool GoalHasDueDate { get; set; }
        DateTime GoalEndDate { get; set; }
        int GoalNotificationIntervalIndex { get; set; }
        GoalAppointmentInterval GoalNotificationInterval { get; }
        TimeSpan GoalNotificationTime { get; set; }
        string GoalImage { get; set; }
        Task LoadGoalsAsync();
        Task<Goal> AddGoalAsync(Goal goal, GoalTask[] goalTasks, string username);
        Task<Goal> EditGoalAsync(Goal goal, GoalTask[] goalTasks, string username);
        Task<GoalTask[]> LoadTasksAsync(Goal parent);
        Task<bool> DeleteGoalAsync(Goal goal);
        void SetGoal(Goal goal);
    }
}