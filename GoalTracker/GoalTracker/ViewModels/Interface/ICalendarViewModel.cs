using System;
using System.Windows.Input;
using Syncfusion.SfCalendar.XForms;

namespace GoalTracker.ViewModels.Interface
{
    public interface ICalendarViewModel
    {
        CalendarEventCollection CalendarInlineEvents { get; set; }
        int SuccessApprovalsWeek { get; set; }
        int FailureApprovalsWeek { get; set; }
        DateTime CalendarMinDate { get; }
        DateTime CalendarMaxDate { get; }

        public ICommand LoadEventsAsyncCommand { get; }
    }
}