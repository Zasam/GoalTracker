using System;
using System.Collections.Generic;
using System.Linq;
using GoalTracker.Entities;
using Xamarin.Forms;

namespace GoalTracker.Extensions
{
    public static class ModelExtensions
    {
        public static int GetGoalIntervalInMilliseconds(this Goal goal)
        {
            return goal.GoalAppointmentInterval switch
            {
                GoalAppointmentInterval.Halbstündlich => 1000 * 60 * 30,
                GoalAppointmentInterval.Stündlich => 1000 * 60 * 60,
                GoalAppointmentInterval.Halbtäglich => 1000 * 60 * 60 * 12,
                GoalAppointmentInterval.Täglich => 1000 * 60 * 60 * 24,
                GoalAppointmentInterval.Wöchentlich => 1000 * 60 * 60 * 24 * 7,
                _ => throw new Exception(
                    "Goal Notification Interval not registered in model extension class (default)")
            };
        }

        public static int GetGoalSuccessStreak(this IEnumerable<GoalAppointment> goalAppointments)
        {
            var successStreak = 0;
            var appointments = goalAppointments as GoalAppointment[] ?? goalAppointments.ToArray();
            if (appointments.Any(gd => gd.Success.HasValue && !gd.Success.Value))
            {
                var sortedGoalDates = appointments.OrderBy(gd => gd.ApprovalDate).ToArray();
                var lastFailure = sortedGoalDates.Last(gd => gd.Success.HasValue && !gd.Success.Value);
                var lastFailureDate = lastFailure.ApprovalDate;
                successStreak = sortedGoalDates.Count(gd =>
                    gd.Success.HasValue && gd.Success.Value && gd.ApprovalDate >= lastFailureDate);
            }
            else
            {
                successStreak = appointments.Count();
            }

            return successStreak;
        }

        public static DateTime? GetLastGoalApproval(this IEnumerable<GoalAppointment> goalAppointments)
        {
            var appointments = goalAppointments as GoalAppointment[] ?? goalAppointments.ToArray();
            if (appointments.Any())
                return appointments.OrderBy(gd => gd.ApprovalDate).Last().ApprovalDate;

            return null;
        }

        public static Color GetColor(this GoalAppointment goalAppointment)
        {
            var approved = goalAppointment.Approved;
            var succeeded = goalAppointment.Success;

            //TODO: Improve color scheme here!
            if (!approved)
                return Color.LightSlateGray;

            if (succeeded.HasValue)
                return succeeded.Value ? Color.ForestGreen : Color.DarkRed;

            return Color.DarkSlateGray;
        }

        public static IEnumerable<GoalAppointment> GetAppointments(this Goal parent)
        {
            var goalAppointments = new List<GoalAppointment>();

            var interval = parent.GoalAppointmentInterval;
            //TODO: How to set end date correctly? Regenerate goalAppointments after some time?
            var endDate = parent.HasDueDate ? parent.EndDate : parent.StartDate.AddMonths(1);
            for (var intervalDate = parent.StartDate;
                intervalDate <= endDate;
                intervalDate = intervalDate.AddMilliseconds(interval.GetMilliseconds()))
            {
                var goalAppointment = new GoalAppointment(parent, intervalDate);
                goalAppointments.Add(goalAppointment);
            }

            return goalAppointments;
        }

        private static int GetMilliseconds(this GoalAppointmentInterval interval)
        {
            switch (interval)
            {
                case GoalAppointmentInterval.Halbstündlich:
                    return 1000 * 60 * 30;
                case GoalAppointmentInterval.Stündlich:
                    return 1000 * 60 * 60;
                case GoalAppointmentInterval.Halbtäglich:
                    return 1000 * 60 * 60 * 12;
                case GoalAppointmentInterval.Täglich:
                    return 1000 * 60 * 60 * 24;
                case GoalAppointmentInterval.Wöchentlich:
                    return 1000 * 60 * 60 * 24 * 7;
                default:
                    return 1000 * 60 * 60;
            }
        }
    }
}