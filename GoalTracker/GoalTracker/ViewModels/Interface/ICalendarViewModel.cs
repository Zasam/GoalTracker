using System;
using System.Threading.Tasks;
using Syncfusion.SfCalendar.XForms;

namespace GoalTracker.ViewModels.Interface
{
    public interface ICalendarViewModel
    {
        public CalendarEventCollection CalendarInlineEvents { get; }
        public int SuccessApprovalsWeek { get; }
        public int FailureApprovalsWeek { get; }
        public DateTime CalendarMinDate { get; }
        public DateTime CalendarMaxDate { get; }
        public Task LoadEventsAsync();
    }
}