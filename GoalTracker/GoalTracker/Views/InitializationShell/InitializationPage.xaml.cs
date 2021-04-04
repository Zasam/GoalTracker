using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.InitializationShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializationPage : ContentPage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly IUserRepository userRepository;

        private bool isLoaded;

        public InitializationPage(IUserRepository userRepository, IAchievementRepository achievementRepository)
        {
            this.userRepository = userRepository;
            this.achievementRepository = achievementRepository;

            InitializeComponent();
        }

        protected override async void LayoutChildren(double x, double y, double width, double height)
        {
            try
            {
                base.LayoutChildren(x, y, width, height);

                await Task.Delay(5000);

                if (!isLoaded)
                {
                    isLoaded = true;

                    await foreach (var progress in CreateDefaultUserAsync())
                    {
                        InitializationMessage.Text = progress.Item1;
                        InitializationProgressBar.SetProgress(progress.Item2, 1000, Easing.Linear);
                        await Task.Delay(1000);
                    }

                    App.Instance.ChangeToConfigurationShell();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async IAsyncEnumerable<Tuple<string, int>> CreateDefaultUserAsync()
        {
            yield return new Tuple<string, int>("Datenbank wird initialisiert...", 33);
            var user = new User("Default");
            await userRepository.AddAsync(user);

            yield return new Tuple<string, int>("Erfolge werden erstellt...", 66);
            await CreateAchievementsAsync(user);

            yield return new Tuple<string, int>("Daten werden überprüft...", 100);
        }

        //TODO: Move to separate viewmodel! (Remove logic from ui code)
        private async Task CreateAchievementsAsync(User user)
        {
            try
            {
                var signupAchievement = new Achievement(user, "SIGNUP", "Erfolgreich registriert 💯",
                    "Vielen Dank das du " + nameof(GoalTracker) + " installiert hast 💖", 15);
                var firstGoalCreatedAchievement = new Achievement(user, "GOALADD", "Dein erstes Ziel 🚀 erstellt",
                    "Du hast dein erstes Ziel gesetzt, Super!", 25);
                var firstGoalEditedAchievement = new Achievement(user, "GOALEDIT", "Dein erstes Ziel 🚀 bearbeitet",
                    "Du hast dein erstes Ziel bearbeitet", 10);
                var successApproval10Reached = new Achievement(user, "GOALSUCCESSAPPROVAL10", "10x erfolgreich",
                    "Du hast dein Ziel schon 10 mal erfoglreich abgeschlossen", 30);
                var successApproval25Reached = new Achievement(user, "GOALSUCCESSAPPROVAL25", "25x erfolgreich",
                    "Du hast dein Ziel schon 25 mal erfolgreich abgeschlossen", 45);
                var successApproval50Reached = new Achievement(user, "GOALSUCCESSAPPROVAL50", "50x erfolgreich",
                    "Du hast dein Ziel schon 50 mal erfolgreich abgeschlossen", 60);
                var approval10Reached = new Achievement(user, "GOALAPPROVALGEN10", "10 Benachrichtigung bestätigt",
                    "Vielen Dank das du " + nameof(GoalTracker) + " nutzt! Schön das du an deinen Zielen dran bleibst.",
                    10);
                var approval25Reached = new Achievement(user, "GOALAPPROVALGEN25", "25 Benachrichtigung bestätigt",
                    "Vielen Dank das du " + nameof(GoalTracker) +
                    " nutzt! WoW du hast schon 25 Benachrichtigungen bestätigt.", 25);
                var approval50Reached = new Achievement(user, "GOALAPPROVALGEN50", "50 Benachrichtigung bestätigt",
                    "Vielen Dank das du " + nameof(GoalTracker) +
                    " nutzt! Unglaublich du hast schon 50 Benachrichtigungen bestätigt.", 50);

                var achievements = new List<Achievement>
                {
                    signupAchievement, firstGoalCreatedAchievement, firstGoalEditedAchievement, approval10Reached,
                    approval25Reached, approval50Reached, successApproval10Reached, successApproval25Reached,
                    successApproval50Reached
                };
                await achievementRepository.AddRangeAsync(achievements);
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