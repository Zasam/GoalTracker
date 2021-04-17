using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Extensions;
using GoalTracker.Models;
using GoalTracker.Services;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.SfCalendar.XForms;

namespace GoalTracker.ViewModels
{
    public class CalendarViewModel : BaseViewModel, ICalendarViewModel
    {
        private readonly IGoalAppointmentRepository goalAppointmentRepository;
        private readonly IGoalRepository goalRepository;
        private CalendarEventCollection calendarInlineEvents;

        private int failureApprovalsWeek;
        private int successApprovalsWeek;

        public CalendarViewModel(IGoalRepository goalRepository, IGoalAppointmentRepository goalAppointmentRepository)
        {
            this.goalRepository = goalRepository;
            this.goalAppointmentRepository = goalAppointmentRepository;
            var now = DateTime.Now;
            CalendarMinDate = new DateTime(now.Year - 1, now.Month, now.Day);
            CalendarMaxDate = new DateTime(now.Year + 1, now.Month, now.Day);
        }

        public CalendarEventCollection CalendarInlineEvents
        {
            get => calendarInlineEvents;
            set
            {
                calendarInlineEvents = value;
                OnPropertyChanged();
            }
        }

        public int SuccessApprovalsWeek
        {
            get => successApprovalsWeek;
            set
            {
                successApprovalsWeek = value;
                OnPropertyChanged();
            }
        }

        public int FailureApprovalsWeek
        {
            get => failureApprovalsWeek;
            set
            {
                failureApprovalsWeek = value;
                OnPropertyChanged();
            }
        }

        public DateTime CalendarMinDate { get; set; }
        public DateTime CalendarMaxDate { get; set; }

        public async Task LoadEventsAsync()
        {
            try
            {
                CalendarInlineEvents = new CalendarEventCollection();
                var goals = await goalRepository.GetAllAsync();

                //TODO: Limit amount of calendarInlineEvents!! Above 1000 are definitely be too much!
                foreach (var goal in goals)
                {
                    var indexCounter = 0;
                    foreach (var goalAppointment in goal.GoalAppointments)
                    {
                        var calendarEvent = new GoalCalendarInlineEvent(goal, goalAppointment, indexCounter);
                        CalendarInlineEvents.Add(calendarEvent);
                        indexCounter++;
                    }
                }

                await CalculateApprovals();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task CalculateApprovals()
        {
            try
            {
                successApprovalsWeek = 0;
                failureApprovalsWeek = 0;

                var goalAppointments = await goalAppointmentRepository.GetAllAsync();

                foreach (var goalDate in goalAppointments.Where(gd => gd.ApprovalDate.HasValue &&
                                                                      gd.ApprovalDate.Value.GetWeekOfYear() ==
                                                                      DateTime.Now.GetWeekOfYear()))
                    if (goalDate.Success.HasValue && goalDate.Success.Value)
                        successApprovalsWeek++;
                    else if (goalDate.Success.HasValue && !goalDate.Success.Value)
                        failureApprovalsWeek++;

                SuccessApprovalsWeek = successApprovalsWeek;
                FailureApprovalsWeek = failureApprovalsWeek;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}