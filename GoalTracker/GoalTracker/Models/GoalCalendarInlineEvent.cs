using System;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using Microsoft.AppCenter.Crashes;
using Syncfusion.SfCalendar.XForms;

namespace GoalTracker.Models
{
    public class GoalCalendarInlineEvent : CalendarInlineEvent
    {
        private CalendarEventState eventState;

        public GoalCalendarInlineEvent()
        {
        }

        public GoalCalendarInlineEvent(Goal parent, GoalAppointment goalAppointment, int index)
        {
            try
            {
                var startDate = new DateTime(parent.StartDate.Year, parent.StartDate.Month, parent.StartDate.Day,
                    parent.NotificationTime.Hours, parent.NotificationTime.Minutes, 00);

                var parentIntervalMs = parent.GetGoalIntervalInMilliseconds();
                var startDateTime = startDate.AddMilliseconds(parentIntervalMs * index);
                var endDateTime = startDate.AddMilliseconds(parentIntervalMs * index + 3600000);
                StartTime = startDateTime;
                EndTime = endDateTime;
                Subject = parent.Title;
                Color = goalAppointment.GetColor();
                //TODO: Is SuccessfulDay properly set to a value?
                SuccessfulDay = true;
                EventState = new CalendarEventState(goalAppointment.Approved, goalAppointment.Success ?? false);
                DateSpan = "Beginnt um " + StartTime.ToString("HH:mm") + " Uhr" + " | Interval: " +
                           parent.GoalAppointmentInterval;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public bool SuccessfulDay { get; set; }

        public string DateSpan { get; set; }

        public CalendarEventState EventState
        {
            get => eventState;
            set
            {
                eventState = value;
                OnPropertyChanged();
            }
        }

        public string Day { get; set; }
    }

    public struct CalendarEventState
    {
        public bool Approved { get; set; }
        public bool Success { get; set; }

        public CalendarEventState(bool approved, bool success)
        {
            Approved = approved;
            Success = success;
        }
    }
}