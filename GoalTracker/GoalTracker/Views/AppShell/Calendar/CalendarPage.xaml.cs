using System;
using System.Globalization;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Calendar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        private readonly IGoalAppointmentRepository goalDateRepository;
        private readonly ICalendarViewModel viewModel;

        public CalendarPage(ICalendarViewModel viewModel, IGoalAppointmentRepository goalDateRepository)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.goalDateRepository = goalDateRepository;

            GoalReminderCalendar.Locale = new CultureInfo("de-DE");
            BindingContext = this.viewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                viewModel.GenerateCalendarEvents();
                GoalReminderCalendar.Refresh();

                base.OnAppearing();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
                Crashes.TrackError(ex);
            }
        }
    }
}