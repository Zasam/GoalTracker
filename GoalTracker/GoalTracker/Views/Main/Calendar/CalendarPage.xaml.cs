using System;
using System.Globalization;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage(ICalendarViewModel calendarViewModel)
        {
            InitializeComponent();

            GoalReminderCalendar.Locale = new CultureInfo("de-DE");
            BindingContext = calendarViewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                GoalReminderCalendar.Refresh();
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}