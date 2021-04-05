using System;
using Autofac;
using BQFramework.Tasks;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker
{
    public partial class App : Application
    {
        public static App Instance;
        private readonly IAchievementRepository achievementRepository;
        private readonly ICalendarViewModel calendarViewModel;
        private readonly IGoalAppointmentRepository goalDateRepository;
        private readonly IGoalRepository goalRepository;
        private readonly IGoalTaskRepository goalTaskRepository;

        private readonly IGoalViewModel goalViewModel;
        private readonly IRegistrationViewModel registrationViewModel;
        private readonly ISettingsViewModel settingsViewModel;
        private readonly IUserRepository userRepository;
        public IContainer container;

        public App(IContainer container)
        {
            try
            {
                this.container = container;

                // TODO: Implementation of theme changing is missing! ThemeManager.ChangeTheme(ThemeManager.Themes.Dark); => Specific android implementation is missing: https://medium.com/@milan.gohil/adding-themes-to-your-xamarin-forms-app-3da3032cc3a1
                ThemeManager.LoadTheme();

                InitializeComponent();

                // Resolve needed repositories
                goalRepository = container.Resolve<IGoalRepository>();
                goalDateRepository = container.Resolve<IGoalAppointmentRepository>();
                userRepository = container.Resolve<IUserRepository>();
                achievementRepository = container.Resolve<IAchievementRepository>();
                goalTaskRepository = container.Resolve<IGoalTaskRepository>();

                // Resolve needed viewmodels
                goalViewModel = container.Resolve<IGoalViewModel>();
                calendarViewModel = container.Resolve<ICalendarViewModel>();
                settingsViewModel = container.Resolve<ISettingsViewModel>();
                registrationViewModel = container.Resolve<IRegistrationViewModel>();

                Instance = this;

                var user = AsyncHelper.RunSync(() => userRepository.GetAsync(1));

                if (user == null)
                    MainPage = new InitializationShell(userRepository, achievementRepository);
                else if (user.Name == "Default")
                    MainPage = new AppConfigurationShell(userRepository, achievementRepository, registrationViewModel);
                else
                    MainPage = new AppShell(goalRepository, goalDateRepository, userRepository, achievementRepository,
                        goalTaskRepository,
                        goalViewModel, registrationViewModel, calendarViewModel, settingsViewModel);
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
                    MainPage = new AppConfigurationShell(userRepository, achievementRepository, registrationViewModel);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void ChangeToAppShell()
        {
            try
            {
                if (MainPage.GetType() == typeof(AppConfigurationShell))
                    MainPage = new AppShell(goalRepository, goalDateRepository, userRepository, achievementRepository,
                        goalTaskRepository,
                        goalViewModel, registrationViewModel, calendarViewModel, settingsViewModel);
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