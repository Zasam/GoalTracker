using System;
using System.Collections.Generic;
using System.Linq;
using GoalTracker.Entities;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Extensions
{
    public static class ModelExtensions
    {
        public static int GetGoalIntervalInMilliseconds(this Goal goal)
        {
            try
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public static int GetGoalSuccessStreak(this IEnumerable<GoalAppointment> goalAppointments)
        {
            try
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 0;
            }
        }

        public static DateTime? GetLastGoalApproval(this IEnumerable<GoalAppointment> goalAppointments)
        {
            try
            {
                var appointments = goalAppointments as GoalAppointment[] ?? goalAppointments.ToArray();
                if (appointments.Any())
                    return appointments.OrderBy(gd => gd.ApprovalDate).Last().ApprovalDate;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return null;
        }

        public static Color GetColor(this GoalAppointment goalAppointment)
        {
            try
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return Color.DarkRed;
            }
        }

        public static IEnumerable<GoalAppointment> GetAppointments(this Goal parent)
        {
            try
            {
                var goalAppointments = new List<GoalAppointment>();

                var interval = parent.GoalAppointmentInterval;
                //TODO: How to set end date correctly? Regenerate goalAppointments after some time?
                var endDate = parent.HasDueDate ? parent.EndDate : parent.StartDate.AddMonths(1);
                for (var intervalDate = new DateTime(parent.StartDate.Year, parent.StartDate.Month, parent.StartDate.Day, parent.NotificationTime.Hours, parent.NotificationTime.Minutes, 00);
                    intervalDate <= endDate;
                    intervalDate = intervalDate.AddMilliseconds(interval.GetMilliseconds()))
                {
                    var goalAppointment = new GoalAppointment(parent, intervalDate);
                    goalAppointments.Add(goalAppointment);
                }

                return goalAppointments;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        private static int GetMilliseconds(this GoalAppointmentInterval interval)
        {
            try
            {
                return interval switch
                {
                    GoalAppointmentInterval.Halbstündlich => 1000 * 60 * 30,
                    GoalAppointmentInterval.Stündlich => 1000 * 60 * 60,
                    GoalAppointmentInterval.Halbtäglich => 1000 * 60 * 60 * 12,
                    GoalAppointmentInterval.Täglich => 1000 * 60 * 60 * 24,
                    GoalAppointmentInterval.Wöchentlich => 1000 * 60 * 60 * 24 * 7,
                    _ => 1000 * 60 * 60
                };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return 1000 * 60 * 60 * 24;
            }
        }
    }
}