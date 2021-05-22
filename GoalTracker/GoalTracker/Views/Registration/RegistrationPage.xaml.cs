using System;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
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
                {
                    DependencyService.Get<IMessenger>().ShortMessage("Bitte gib deinen Vornamen ein um fortzufahren");
                    return;
                }

                //TODO: How to get title of the unlocked achievement?
                await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel, AchievementProgressBar, "Erfolg freigeschaltet: " + settingViewModel.LoadedAchievement.Title);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void AchievementProgressBar_OnProgressCompleted(object sender, ProgressValueEventArgs e)
        {
            AppShell.Instance.SetUIState(UIStates.Configuration, UIStates.Home);
        }
    }
}