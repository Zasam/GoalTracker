using System;
using System.Globalization;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        private readonly ICalendarViewModel calendarViewModel;

        public CalendarPage(ICalendarViewModel calendarViewModel)
        {
            InitializeComponent();

            this.calendarViewModel = calendarViewModel;

            GoalReminderCalendar.Locale = new CultureInfo("de-DE");
            BindingContext = this.calendarViewModel;
        }

        protected override async void OnAppearing()
        {
            try
            {
                await calendarViewModel.LoadEventsAsync();
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