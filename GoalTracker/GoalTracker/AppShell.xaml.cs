using System;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using GoalTracker.Views.AppShell.Calendar;
using GoalTracker.Views.AppShell.Goals;
using GoalTracker.Views.AppShell.Settings;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    /// <summary>
    ///     TODO: Add system so earn XP when leveling up goals (multiple wins)
    /// </summary>
    public partial class AppShell : Shell
    {
        public AppShell(IGoalRepository goalRepository, IGoalAppointmentRepository goalDateRepository,
            IUserRepository userRepository, IAchievementRepository achievementRepository,
            IGoalTaskRepository goalTaskRepository, IGoalViewModel goalViewModel,
            IRegistrationViewModel registrationViewModel, ICalendarViewModel calendarViewModel,
            ISettingsViewModel settingsViewModel)
        {
            try
            {
                InitializeComponent();

                var username = userRepository.GetUsernameAsync().Result;

                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Ziele",
                    Icon = "Goal.png",
                    Content = new GoalsPage(goalRepository, goalDateRepository, achievementRepository, goalViewModel,
                        goalTaskRepository, username),
                    Route = "GoalsPage"
                });

                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Kalender",
                    Icon = "Calendar.png",
                    Content = new CalendarPage(calendarViewModel, goalDateRepository),
                    Route = "CalendarPage"
                });

                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Einstellungen",
                    Icon = "Settings.png",
                    Content = new SettingsPage(userRepository, achievementRepository, settingsViewModel),
                    Route = "SettingsPage"
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//GoalsPage");
        }
    }
}