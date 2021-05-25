using System;
using GoalTracker.Entities;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class App : Application
    {
        public App(User user, IGoalViewModel goalViewModel, ICalendarViewModel calendarViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                InitializeComponent();

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