using System;
using System.Linq;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.RegistrationShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly IUserRepository userRepository;
        private readonly IRegistrationViewModel viewModel;

        public RegistrationPage(IUserRepository userRepository, IAchievementRepository achievementRepository,
            IRegistrationViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.userRepository = userRepository;
            this.achievementRepository = achievementRepository;

            BindingContext = viewModel;
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
                if (string.IsNullOrWhiteSpace(viewModel.Username))
                    //TODO: Show validation message to user!
                    return;

                var defaultUser = await userRepository.GetAsync(1);

                if (defaultUser == null)
                    throw new InvalidOperationException(
                        "Default initialization has not been started or has not been completed");

                defaultUser.Name = viewModel.Username;
                await userRepository.SaveChangesAsync();

                var unlockableAchievements = await achievementRepository.FindAsync(a => a.InternalTag == "SIGNUP");
                var unlockableAchievement = unlockableAchievements.FirstOrDefault();

                if (unlockableAchievement != null)
                {
                    unlockableAchievement.Unlock();
                    await achievementRepository.SaveChangesAsync();

                    //TODO: Implement progress bar not only for achievement unlocked, but (also) for progress of signup!
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