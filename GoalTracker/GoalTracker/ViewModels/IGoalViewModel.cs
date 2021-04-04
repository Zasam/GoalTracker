using System;
using System.Collections.Generic;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels
{
    public interface IGoalViewModel
    {
        /// <summary>
        ///     The goals from the database which are binded to
        /// </summary>
        public List<Goal> Goals { get; set; }

        public List<string> GoalNotificationIntervals { get; set; }

        public Goal SelectedGoal { get; set; }

        /// <summary>
        ///     The binded title of the goal to create
        /// </summary>
        public string GoalTitle { get; set; }

        public string GoalNotes { get; set; }

        /// <summary>
        ///     The minimum start date of the goal to create
        /// </summary>
        public DateTime GoalMinimumStartDate { get; set; }

        /// <summary>
        ///     The minimum end date of the goal to create
        /// </summary>
        public DateTime GoalMinimumEndDate { get; set; }

        /// <summary>
        ///     The binded start date of the goal to create
        /// </summary>
        public DateTime GoalStartDate { get; set; }

        /// <summary>
        ///     The binded indication if the goal to create has an end date
        /// </summary>
        public bool GoalHasDueDate { get; set; }

        /// <summary>
        ///     The binded end date of the goal to create
        /// </summary>
        public DateTime GoalEndDate { get; set; }

        public int GoalNotificationIntervalIndex { get; set; }

        public GoalAppointmentInterval GoalNotificationInterval { get; }

        /// <summary>
        ///     The binded first notification time of the goal to create
        /// </summary>
        public TimeSpan GoalNotificationTime { get; set; }

        public double AchievementProgress { get; set; }
        public int AchievementProgressPoints { get; set; }
    }
}