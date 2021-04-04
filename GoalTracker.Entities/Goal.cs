using System;
using System.Collections.Generic;

namespace GoalTracker.Entities
{
    public class Goal : BaseEntity
    {
        public Goal()
        {
            Title = string.Empty;
            Notes = string.Empty;
            StartDate = DateTime.Now;
            EndDate = DateTime.MaxValue;
            GoalAppointmentInterval = GoalAppointmentInterval.Stündlich;
            NotificationTime = TimeSpan.FromHours(8);
            NotificationId = 1;
            RequestCode = 1;
        }

        /// <summary>
        ///     Initialize a new goal instance
        /// </summary>
        /// <param name="title">Title of the goal</param>
        /// <param name="notes">Notes of the goal</param>
        /// <param name="startDate">Start date of the goal</param>
        /// <param name="hasDueDate">Indication if the goal has an end date</param>
        /// <param name="endDate">End date of the goal</param>
        /// <param name="goalAppointmentInterval">Appointment notification interval of the goal</param>
        /// <param name="notificationTime">First notification time of the goal</param>
        /// <param name="notificationId">The notification id of the goal</param>
        /// <param name="startingRequestCode">The starting request code of the goal</param>
        public Goal(string title, string notes, DateTime startDate, bool hasDueDate, DateTime endDate,
            GoalAppointmentInterval goalAppointmentInterval, TimeSpan notificationTime, int notificationId,
            int startingRequestCode)
        {
            Title = title;
            Notes = notes;
            StartDate = startDate;
            HasDueDate = hasDueDate;
            EndDate = endDate;
            GoalAppointmentInterval = goalAppointmentInterval;
            NotificationTime = notificationTime;
            NotificationId = notificationId;
            RequestCode = startingRequestCode;
        }

        #region Properties

        /// <summary>
        ///     Title describing the goal
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Notes describing the goal more intensively
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        ///     Date when tracking of the goal should start
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Indicates whether the goal has a end date or not
        /// </summary>
        public bool HasDueDate { get; set; }

        /// <summary>
        ///     Date when tracking of the goal should end
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     The interval to notify the user of the goal
        /// </summary>
        public GoalAppointmentInterval GoalAppointmentInterval { get; set; }

        public virtual IEnumerable<GoalAppointment> GoalAppointments { get; set; }

        public virtual IEnumerable<GoalTask> GoalTasks { get; set; }

        public int GoalTaskCount { get; set; }

        public bool AllTasksCompleted { get; set; }

        #region Notification

        /// <summary>
        ///     The time when the first notification should be triggered
        /// </summary>
        public TimeSpan NotificationTime { get; set; }

        public int NotificationId { get; set; }

        public int RequestCode { get; set; }

        #endregion Notification

        #endregion // Properties
    }
}