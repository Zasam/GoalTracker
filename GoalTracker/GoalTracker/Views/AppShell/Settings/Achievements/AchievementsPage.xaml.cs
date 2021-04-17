using System;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Settings.Achievements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;

        public AchievementsPage(ISettingViewModel settingViewModel)
        {
            this.settingViewModel = settingViewModel;
            BindingContext = settingViewModel;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                await settingViewModel.LoadAchievementsAsync();
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}