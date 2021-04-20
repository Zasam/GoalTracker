using System;
using GoalTracker.Entities;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.AppShell.Calendar;
using GoalTracker.Views.AppShell.Home.Goals;
using GoalTracker.Views.AppShell.Settings;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class AppShell : Shell
    {
        public AppShell(User user, IGoalViewModel goalViewModel, ICalendarViewModel calendarViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                InitializeComponent();

                //TODO: Change to routing?
                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Ziele",
                    Icon = "Goal.png",
                    Content = new GoalsPage(goalViewModel, settingViewModel, user.Name),
                    Route = "GoalsPage"
                });

                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Kalender",
                    Icon = "Calendar.png",
                    Content = new CalendarPage(calendarViewModel),
                    Route = "CalendarPage"
                });

                Tabbar.Items.Add(new ShellContent
                {
                    Title = "Einstellungen",
                    Icon = "Settings.png",
                    Content = new SettingsPage(settingViewModel),
                    Route = "SettingsPage"
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}