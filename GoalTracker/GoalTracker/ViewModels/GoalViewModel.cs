using System;
using System.Collections.Generic;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels
{
    public class GoalViewModel : BaseViewModel, IGoalViewModel
    {
        private double achievementProgress;

        private int achievementProgressPoints;

        private bool goalHasDueDate;
        private List<Goal> goals;

        public GoalViewModel()
        {
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

        public List<string> GoalNotificationIntervals { get; set; }

        public string GoalTitle { get; set; }
        public string GoalNotes { get; set; }
        public DateTime GoalMinimumStartDate { get; set; }
        public DateTime GoalMinimumEndDate { get; set; }
        public DateTime GoalStartDate { get; set; }

        public bool GoalHasDueDate
        {
            get => goalHasDueDate;
            set
            {
                goalHasDueDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime GoalEndDate { get; set; }

        public int GoalNotificationIntervalIndex { get; set; }

        public GoalAppointmentInterval GoalNotificationInterval =>
            (GoalAppointmentInterval) GoalNotificationIntervalIndex;

        public TimeSpan GoalNotificationTime { get; set; }

        public double AchievementProgress
        {
            get => achievementProgress;
            set
            {
                achievementProgress = value;
                OnPropertyChanged();
            }
        }

        public int AchievementProgressPoints
        {
            get => achievementProgressPoints;
            set
            {
                achievementProgressPoints = value;
                OnPropertyChanged();
            }
        }
    }
}