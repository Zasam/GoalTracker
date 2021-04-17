using System;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.AppShell.Settings.Achievements;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Settings
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
            Navigation.PushAsync(new AchievementsPage(settingViewModel), true);
        }
    }
}