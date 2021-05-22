using System;
using GoalTracker.Entities;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class App : Application
    {
        private readonly IGoalViewModel goalViewModel;
        private readonly ICalendarViewModel calendarViewModel;
        private readonly ISettingViewModel settingViewModel;

        public App(User user, IGoalViewModel goalViewModel, ICalendarViewModel calendarViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                InitializeComponent();

                this.goalViewModel = goalViewModel;
                this.calendarViewModel = calendarViewModel;
                this.settingViewModel = settingViewModel;

                // TODO: Implementation of theme changing is missing! ThemeManager.ChangeTheme(ThemeManager.Themes.Dark); => Specific android implementation is missing: https://medium.com/@milan.gohil/adding-themes-to-your-xamarin-forms-app-3da3032cc3a1
                ThemeManager.LoadTheme();

                MainPage = new AppShell(user, goalViewModel, calendarViewModel, settingViewModel);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}