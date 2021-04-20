using System;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class App : Application
    {
        public static App Instance;

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

                Instance = this;

                // TODO: Implementation of theme changing is missing! ThemeManager.ChangeTheme(ThemeManager.Themes.Dark); => Specific android implementation is missing: https://medium.com/@milan.gohil/adding-themes-to-your-xamarin-forms-app-3da3032cc3a1
                ThemeManager.LoadTheme();

                if (user == null)
                    MainPage = new InitializationShell(settingViewModel);
                else if (user.Name == "Default")
                    MainPage = new AppConfigurationShell(settingViewModel);
                else
                    MainPage = new AppShell(user, goalViewModel, calendarViewModel, settingViewModel);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void ChangeToConfigurationShell()
        {
            try
            {
                if (MainPage.GetType() == typeof(InitializationShell))
                    MainPage = new AppConfigurationShell(settingViewModel);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task ChangeToAppShell()
        {
            try
            {
                if (MainPage.GetType() == typeof(AppConfigurationShell))
                {
                    await settingViewModel.LoadUserAsync();
                    MainPage = new AppShell(settingViewModel.User, goalViewModel, calendarViewModel, settingViewModel);
                }
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