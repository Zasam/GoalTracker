using System;
using GoalTracker.Extensions;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Registration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;

        public RegistrationPage(ISettingViewModel settingViewModel)
        {
            InitializeComponent();

            BindingContext = this.settingViewModel = settingViewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                AchievementStackLayout.InitializeAchievementAnimation();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void FinishAppConfigurationButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                //TODO: Show validation message to user!
                if (string.IsNullOrWhiteSpace(settingViewModel.Username))
                    return;

                var unlockableAchievement = await settingViewModel.GetAchievementAsync("SIGNUP");

                if (unlockableAchievement != null)
                {
                    await settingViewModel.ChangeUsername(settingViewModel.Username);
                    await settingViewModel.UnlockAchievementAsync("SIGNUP");
                    await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel, AchievementProgressBar, "Erfolg freigeschaltet: " + unlockableAchievement.Title);
                }
                else
                {
                    await settingViewModel.LoadUserAsync();
                    GoalTracker.AppShell.Instance.SetUIState(settingViewModel.User, UIStates.Configuration, UIStates.Home);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void AchievementProgressBar_OnProgressCompleted(object sender, ProgressValueEventArgs e)
        {
            await settingViewModel.LoadUserAsync();
            GoalTracker.AppShell.Instance.SetUIState(settingViewModel.User, UIStates.Configuration, UIStates.Home);
        }
    }
}