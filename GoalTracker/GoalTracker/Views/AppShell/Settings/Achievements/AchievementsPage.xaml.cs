using System;
using System.Linq;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Settings.Achievements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsPage : ContentPage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly ISettingsViewModel viewModel;

        public AchievementsPage(IAchievementRepository achievementRepository, ISettingsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.achievementRepository = achievementRepository;

            BindingContext = viewModel;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                var achievements = await achievementRepository.GetAllAsync();
                viewModel.Achievements = achievements.ToList();

                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }
    }
}