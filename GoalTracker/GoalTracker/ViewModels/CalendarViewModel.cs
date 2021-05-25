using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Extensions;
using GoalTracker.Models;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.SfCalendar.XForms;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class CalendarViewModel : BaseViewModel, ICalendarViewModel
    {
        #region Repositories

        private readonly IGoalRepository goalRepository;
        private readonly IGoalAppointmentRepository goalAppointmentRepository;

        #endregion // Repositories

        #region Properties

        private CalendarEventCollection calendarInlineEvents;

        /// <summary>
        /// Calendar events with appointments which can be binded to a syncfusion calendar
        /// </summary>
        public CalendarEventCollection CalendarInlineEvents
        {
            get => calendarInlineEvents;
            set
            {
                calendarInlineEvents = value;
                OnPropertyChanged();
            }
        }

        private int successApprovalsWeek;

        /// <summary>
        /// Count of appointments which were approved with success this week
        /// </summary>
        public int SuccessApprovalsWeek
        {
            get => successApprovalsWeek;
            set
            {
                successApprovalsWeek = value;
                OnPropertyChanged();
            }
        }

        private int failureApprovalsWeek;

        /// <summary>
        /// Count of appointments which were approved with failure this week
        /// </summary>
        public int FailureApprovalsWeek
        {
            get => failureApprovalsWeek;
            set
            {
                failureApprovalsWeek = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The minimum date of the calendar to show
        /// </summary>
        public DateTime CalendarMinDate { get; }

        /// <summary>
        /// The maximum date of the calendar to show
        /// </summary>
        public DateTime CalendarMaxDate { get; }

        #endregion // ReadOnly Bindings

        #region Commands

        /// <summary>
        /// Command to load all events associated with existing appointments async
        /// </summary>
        public ICommand LoadEventsAsyncCommand { get; }

        #endregion // Commands

        // TODO: Only used to check bindings in xaml
        public CalendarViewModel()
        {
            throw new InvalidOperationException("Calendar view model shouldn't be initialized through parameterless constructor");
        }

        public CalendarViewModel(IGoalRepository goalRepository, IGoalAppointmentRepository goalAppointmentRepository)
        {
            try
            {
                this.goalRepository = goalRepository;
                this.goalAppointmentRepository = goalAppointmentRepository;

                var now = DateTime.Now;
                CalendarMinDate = new DateTime(now.Year - 1, now.Month, now.Day);
                CalendarMaxDate = new DateTime(now.Year + 1, now.Month, now.Day);

                LoadEventsAsyncCommand = new Command(async () => await LoadEventsAsync());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadEventsAsync()
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