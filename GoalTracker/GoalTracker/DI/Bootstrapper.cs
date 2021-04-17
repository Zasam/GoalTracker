using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using GoalTracker.Context;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.DI
{
    public static class Bootstrapper
    {
        private static IContainer container;

        public static async Task<App> Run()
        {
            try
            {
                // Create builder and default context to use
                var builder = new ContainerBuilder();

                // Register the custom database context
                builder.RegisterType<GoalTrackerContext>().As<IGoalTrackerContext>().SingleInstance();

                // Register all repositories
                builder.RegisterType<GoalRepository>().As<IGoalRepository>().SingleInstance();
                builder.RegisterType<GoalAppointmentRepository>().As<IGoalAppointmentRepository>().SingleInstance();
                builder.RegisterType<GoalTaskRepository>().As<IGoalTaskRepository>().SingleInstance();
                builder.RegisterType<AchievementRepository>().As<IAchievementRepository>().SingleInstance();
                builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();

                // Register all viewmodels
                builder.RegisterType<GoalViewModel>().As<IGoalViewModel>().SingleInstance();
                builder.RegisterType<CalendarViewModel>().As<ICalendarViewModel>().SingleInstance();
                builder.RegisterType<SettingViewModel>().As<ISettingViewModel>().SingleInstance();

                container = builder.Build();

                var goalViewModel = container.Resolve<IGoalViewModel>();
                var calendarViewModel = container.Resolve<ICalendarViewModel>();
                var settingViewModel = container.Resolve<ISettingViewModel>();

                // Check if user has already signed up and user exists
                var userRepository = container.Resolve<IUserRepository>();
                var users = await userRepository.GetAllAsync();
                var user = users.FirstOrDefault();

                return new App(user, goalViewModel, calendarViewModel, settingViewModel);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public static async Task<IContainer> GetContainer()
        {
            try
            {
                if (container == null)
                    await Run();

                return container;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }
    }
}