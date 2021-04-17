using System;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.RegistrationShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;

        public RegistrationPage(ISettingViewModel settingViewModel)
        {
            InitializeComponent();

            this.settingViewModel = settingViewModel;
            BindingContext = settingViewModel;
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void FinishAppConfigurationButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(settingViewModel.Username))
                    //TODO: Show validation message to user!
                    return;

                var unlockableAchievement = await settingViewModel.GetAchievementAsync("SIGNUP");

                if (unlockableAchievement != null)
                {
                    await settingViewModel.UnlockAchievementAsync("SIGNUP");
                    await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel,
                        AchievementProgressBar, "Erfolg freigeschaltet: " + unlockableAchievement.Title);
                }
                else
                {
                    App.Instance.ChangeToAppShell();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void AchievementProgressBar_OnProgressCompleted(object sender, ProgressValueEventArgs e)
        {
            App.Instance.ChangeToAppShell();
        }
    }
}