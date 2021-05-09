using System;
using System.Collections.Generic;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalViewModel
    {
        IGoalTaskRepository GoalTaskRepository { get; }
        IGoalAppointmentRepository GoalAppointmentRepository { get; }

        string GoalTitle { get; set; }
        string GoalNotes { get; set; }
        DateTime GoalMinimumStartDate { get; set; }
        DateTime GoalMinimumEndDate { get; set; }
        DateTime GoalStartDate { get; set; }
        DateTime GoalEndDate { get; set; }
        int GoalNotificationIntervalIndex { get; set; }
        TimeSpan GoalNotificationTime { get; set; }
        bool GoalHasDueDate { get; set; }
        GoalAppointmentInterval GoalNotificationInterval { get; }
        string GoalImage { get; set; }
        Goal SelectedGoal { get; set; }
        List<Goal> Goals { get; set; }
        List<string> GoalNotificationIntervals { get; }
        List<string> GoalTaskTitles { get; set; }
        List<GoalTask> GoalTasks { get; set; }
        bool IsRefreshing { get; set; }
        public Goal BindedGoal { get; }
        ISettingViewModel SettingViewModel { get; set; }

        ICommand LoadGoalsAsyncCommand { get; }
        ICommand AddGoalAsyncCommand { get; }
        ICommand EditGoalAsyncCommand { get; }
        ICommand DeleteGoalAsyncCommand { get; }
        ICommand LoadTasksAsyncCommand { get; }
    }
}