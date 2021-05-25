using System;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.Main.Settings.Achievements;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;

        public SettingsPage(ISettingViewModel settingViewModel)
        {
            this.settingViewModel = settingViewModel;
            BindingContext = this.settingViewModel;

            InitializeComponent();
        }

        private void ShowAchievementsButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new AchievementsPage(settingViewModel), true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}