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
            DetailImage = "❔";
            GoalAppointments = new List<GoalAppointment>();
            GoalTasks = new List<GoalTask>();
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
        /// <param name="goalImage">The descriptive image of the goal</param>
        public Goal(string title, string notes, DateTime startDate, bool hasDueDate, DateTime endDate,
            GoalAppointmentInterval goalAppointmentInterval, TimeSpan notificationTime, int notificationId,
            int startingRequestCode, string goalImage)
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
            DetailImage = goalImage;
            GoalAppointments = new List<GoalAppointment>();
            GoalTasks = new List<GoalTask>();
        }

        public Goal(string title, string notes, DateTime startDate, bool hasDueDate, DateTime endDate,
            GoalAppointmentInterval goalAppointmentInterval, TimeSpan notificationTime, string goalImage)
        {
            Title = title;
            Notes = notes;
            StartDate = startDate;
            HasDueDate = hasDueDate;
            EndDate = endDate;
            GoalAppointmentInterval = goalAppointmentInterval;
            NotificationTime = notificationTime;
            DetailImage = goalImage;
            GoalAppointments = new List<GoalAppointment>();
            GoalTasks = new List<GoalTask>();
        }

        public void SetNotificationIndex(int notificationId, int startingRequestCode)
        {
            NotificationId = notificationId;
            RequestCode = startingRequestCode;
        }

        #region Properties

        /// <summary>
        /// Title describing the goal
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Notes describing the goal more intensively
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The single line string presented as detail image in viewcell (preferable only emoticons)
        /// </summary>
        public string DetailImage { get; set; }

        /// <summary>
        /// Date when tracking of the goal should start
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Indicates whether the goal has a end date or not
        /// </summary>
        public bool HasDueDate { get; set; }

        /// <summary>
        /// Date when tracking of the goal should end
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The interval to notify the user of the goal
        /// </summary>
        public GoalAppointmentInterval GoalAppointmentInterval { get; set; }

        /// <summary>
        /// Collection of associated goal appointments
        /// </summary>
        public virtual IEnumerable<GoalAppointment> GoalAppointments { get; }

        /// <summary>
        /// Collection of associated goal tasks
        /// </summary>
        public virtual IEnumerable<GoalTask> GoalTasks { get; }

        /// <summary>
        /// The number of tasks associated with this goal
        /// </summary>
        public int GoalTaskCount { get; set; }

        /// <summary>
        /// Indication if all the associated goal tasks are finished by now
        /// </summary>
        public bool AllTasksCompleted { get; set; }

        #region Notification

        /// <summary>
        /// The time when the first notification should be triggered
        /// </summary>
        public TimeSpan NotificationTime { get; set; }

        /// <summary>
        /// The id to handle notifications associated with this goal
        /// </summary>
        public int NotificationId { get; private set; }

        /// <summary>
        /// The request codes to use with the notifications associated with this goal
        /// </summary>
        public int RequestCode { get; private set; }

        #endregion Notification

        #endregion // Properties
    }
}