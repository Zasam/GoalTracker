using System;
using GoalTracker.Entities;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.Initialization;
using GoalTracker.Views.Main.Calendar;
using GoalTracker.Views.Main.Home.Goals;
using GoalTracker.Views.Main.Settings;
using GoalTracker.Views.Registration;
using GoalTracker.Views.Welcome;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class AppShell : Shell
    {
        private readonly IGoalViewModel goalViewModel;
        private readonly ICalendarViewModel calendarViewModel;
        private readonly ISettingViewModel settingViewModel;
        public static AppShell Instance;

        public AppShell(User user, IGoalViewModel goalViewModel, ICalendarViewModel calendarViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                InitializeComponent();

                this.goalViewModel = goalViewModel;
                this.calendarViewModel = calendarViewModel;
                this.settingViewModel = settingViewModel;

                UIStates currentUIState;

                if (user == null)
                    currentUIState = UIStates.Welcome;
                else if (user.Name == "Default")
                    currentUIState = UIStates.Configuration;
                else
                    currentUIState = UIStates.Home;

                SetUIState(UIStates.None, currentUIState);

                Instance = this;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void SetUIState(UIStates currentState, UIStates newState)
        {
            var user = settingViewModel.User;

            switch (newState)
            {
                case UIStates.Welcome:
                    Tabbar.Items.Add(new ShellContent
                    {
                        Title = "Willkommen",
                        Content = new WelcomePage()
                    });
                    break;
                case UIStates.Initialization:
                    Tabbar.Items.Add(new ShellContent
                    {
                        Title = "Initialisierung",
                        Content = new InitializationPage(settingViewModel)
                    });
                    break;
                case UIStates.Configuration:
                    Tabbar.Items.Add(new ShellContent
                    {
                        Title = "Einrichtung",
                        Content = new RegistrationPage(settingViewModel)
                    });
                    break;
                case UIStates.Home:
                    Tabbar.Items.Add(new ShellContent
                    {
                        Title = "Ziele",
                        Icon = "Goal.png",
                        Content = new GoalsPage(goalViewModel, settingViewModel),
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
                    break;
            }

            switch (currentState)
            {
                case UIStates.Welcome:
                case UIStates.Initialization:
                case UIStates.Configuration:
                    Tabbar.Items.RemoveAt(0);
                    break;
                case UIStates.Home:
                    Tabbar.Items.RemoveAt(0);
                    Tabbar.Items.RemoveAt(1);
                    Tabbar.Items.RemoveAt(2);
                    break;
            }
        }
    }

    public enum UIStates
    {
        None,
        Welcome,
        Initialization,
        Configuration,
        Home
    }
}