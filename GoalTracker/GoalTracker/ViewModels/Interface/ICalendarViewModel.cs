using System;
using System.Threading.Tasks;
using Syncfusion.SfCalendar.XForms;

namespace GoalTracker.ViewModels.Interface
{
    public interface ICalendarViewModel
    {
        public CalendarEventCollection CalendarInlineEvents { get; set; }
        public int SuccessApprovalsWeek { get; set; }
        public int FailureApprovalsWeek { get; set; }
        public DateTime CalendarMinDate { get; set; }
        public DateTime CalendarMaxDate { get; set; }
        public Task LoadEventsAsync();
    }
}