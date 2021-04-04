using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autofac;
using GoalTracker.Entities;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.Converter
{
    public class SyncfusionCalendarDateToColorConverter : IValueConverter
    {
        private List<GoalAppointment> goalAppointments;
        private IGoalAppointmentRepository goalDateRepository;
        private IGoalRepository goalRepository;
        private List<Goal> goals;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var returnValue = Color.Transparent;

                if (value is DateTime date)
                {
                    // Only convert a specific dates from a specific time range to colors, because of performance issues | Maybe change that later on..
                    var startConversionDate = new DateTime(date.Year, date.Month, 1);

                    // Initialization on first conversion date || Min and max conversion dates are the first and last shown dates of the calendar component
                    if (date == startConversionDate)
                    {
                        var container = App.Instance.container;

                        goalRepository ??= container.Resolve<IGoalRepository>();
                        goalDateRepository ??= container.Resolve<IGoalAppointmentRepository>();
                    }

                    goals = goalRepository.GetAllInDateAsync(date).Result.ToList();
                    goalAppointments = goalDateRepository.GetAllByDayAsync(date).Result.ToList();

                    if (goalAppointments != null && goalAppointments.Any())
                    {
                        var success = true;

                        foreach (var goalDate in goalAppointments)
                            if (goalDate.Success.HasValue && !goalDate.Success.Value)
                                success = false;

                        returnValue = success ? Color.Green : Color.Red;
                    }
                    else if (goals != null && goals.Any())
                    {
                        returnValue = Color.LightSlateGray;
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                //TODO: how to asynchronously wait for this in overriden method?
                Crashes.TrackError(ex);
                return Color.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}