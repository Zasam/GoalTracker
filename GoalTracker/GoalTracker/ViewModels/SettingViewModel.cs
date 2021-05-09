using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class SettingViewModel : BaseViewModel, ISettingViewModel
    {
        #region Repositories

        private readonly IUserRepository userRepository;
        private readonly IAchievementRepository achievementRepository;

        #endregion // Repositories

        #region Properties

        private List<Achievement> achievements;

        /// <summary>
        /// Collection of all existing achievements
        /// </summary>
        public List<Achievement> Achievements
        {
            get => achievements;
            set
            {
                achievements = value;
                OnPropertyChanged();
            }
        }

        private double achievementProgress;

        /// <summary>
        /// Total progress percentage of all unlocked achievements
        /// </summary>
        public double AchievementProgress
        {
            get => achievementProgress;
            set
            {
                achievementProgress = value;
                OnPropertyChanged();
            }
        }

        private int achievementProgressPoints;

        /// <summary>
        /// Total sum of points of all unlocked achievements
        /// </summary>
        public int AchievementProgressPoints
        {
            get => achievementProgressPoints;
            set
            {
                achievementProgressPoints = value;
                OnPropertyChanged();
            }
        }

        private string username;

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        private User user;

        /// <summary>
        /// The registered user
        /// </summary>
        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        private Achievement loadedAchievement;

        /// <summary>
        /// The last loaded achievement
        /// </summary>
        public Achievement LoadedAchievement
        {
            get => loadedAchievement;
            set
            {
                loadedAchievement = value;
                OnPropertyChanged();
            }
        }

        private string welcomeMessage;

        public string WelcomeMessage
        {
            get => welcomeMessage;
            set
            {
                welcomeMessage = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        /// <summary>
        /// Command to open a specified link in a browser
        /// </summary>
        public ICommand OpenLinkCommand { get; }

        /// <summary>
        /// Async command to load all achievements into the achievement collection
        /// </summary>
        public ICommand LoadAchievementsAsyncCommand { get; }

        /// <summary>
        /// Async command to load a specific achievement into the loaded achievement
        /// </summary>
        public ICommand LoadAchievementAsyncCommand { get; }

        /// <summary>
        /// Async command to unlock a specified achievement
        /// </summary>
        public ICommand UnlockAchievementAsyncCommand { get; }

        /// <summary>
        /// Async command to register the default user
        /// </summary>
        public ICommand RegisterDefaultUserAsyncCommand { get; }

        /// <summary>
        /// Async command to create the default achievements which should be available to be unlocked
        /// </summary>
        public ICommand CreateAchievementsAsyncCommand { get; }

        /// <summary>
        /// Async command to load the registered user
        /// </summary>
        public ICommand LoadUserAsyncCommand { get; }

        /// <summary>
        /// Async command to change the username to a new specified username
        /// </summary>
        public ICommand ChangeUsernameAsyncCommand { get; }

        #endregion // Commands

        #endregion // Properties

        // TODO: Only used to check bindings in xaml
        public SettingViewModel()
        {
            throw new InvalidOperationException("Setting view model shouldn't be initialized through parameterless constructor");
        }

        public SettingViewModel(IAchievementRepository achievementRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.achievementRepository = achievementRepository;

            OpenLinkCommand = new Command<string>(OpenLink);
            LoadAchievementsAsyncCommand = new Command(async () => await LoadAchievementsAsync());
            LoadAchievementAsyncCommand = new Command<string>(async internalTag => await LoadAchievementAsync(internalTag));
            UnlockAchievementAsyncCommand = new Command<string>(async internalTag => await UnlockAchievementAsync(internalTag));
            RegisterDefaultUserAsyncCommand = new Command(async () => await RegisterDefaultUserAsync());
            CreateAchievementsAsyncCommand = new Command(async () => await CreateAchievementsAsync());
            LoadUserAsyncCommand = new Command(async () => await LoadUserAsync());
            ChangeUsernameAsyncCommand = new Command<string>(async name => await ChangeUsernameAsync(name));
        }

        private void OpenLink(string url)
        {
            Launcher.OpenAsync(url);
        }

        private async Task LoadAchievementAsync(string internalTag)
        {
            try
            {
                LoadedAchievement = await achievementRepository.GetByInternalTag(internalTag);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task UnlockAchievementAsync(string internalTag)
        {
            try
            {
                await LoadAchievementAsync(internalTag);
                if (LoadedAchievement != null)
                {
                    var firstUnlock = LoadedAchievement.Unlock();

                    if (firstUnlock)
                        await achievementRepository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadAchievementsAsync()
        {
            try
            {
                var achievementCollection = await achievementRepository.GetAllAsync();
                Achievements = achievementCollection.ToList();

                double unlockedAchievementsCount = Achievements.Count(a => a.Unlocked);
                double achievementsCount = Achievements.Count;
                AchievementProgress = Math.Round(unlockedAchievementsCount / achievementsCount * 100, 0);
                AchievementProgressPoints = Achievements.Where(a => a.Unlocked).Sum(a => a.Experience);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task RegisterDefaultUserAsync()
        {
            var newUser = new User("Default");
            await userRepository.AddUserAsync(newUser);
            await LoadUserAsync();
        }

        private async Task CreateAchievementsAsync()
        {
            try
            {
                await LoadUserAsync();

                if (User == null)
                    await RegisterDefaultUserAsync();

                var signUpAchievement = new Achievement(User, "SIGNUP", "Erfolgreich registriert 💯", "Vielen Dank das du " + nameof(GoalTracker) + " installiert hast 💖", 15);
                var firstGoalCreatedAchievement = new Achievement(User, "GOALADD", "Dein erstes Ziel 🚀 erstellt", "Du hast dein erstes Ziel gesetzt, Super!", 25);
                var firstGoalEditedAchievement = new Achievement(User, "GOALEDIT", "Dein erstes Ziel 🚀 bearbeitet", "Du hast dein erstes Ziel bearbeitet", 10);
                var successApproval10Reached = new Achievement(User, "GOALSUCCESSAPPROVAL10", "10x erfolgreich", "Du hast dein Ziel schon 10 mal erfolgreich abgeschlossen", 30);
                var successApproval25Reached = new Achievement(User, "GOALSUCCESSAPPROVAL25", "25x erfolgreich", "Du hast dein Ziel schon 25 mal erfolgreich abgeschlossen", 45);
                var successApproval50Reached = new Achievement(User, "GOALSUCCESSAPPROVAL50", "50x erfolgreich", "Du hast dein Ziel schon 50 mal erfolgreich abgeschlossen", 60);
                var approval10Reached = new Achievement(User, "GOALAPPROVALGEN10", "10 Benachrichtigung bestätigt", "Vielen Dank das du " + nameof(GoalTracker) + " nutzt! Schön das du an deinen Zielen dran bleibst.", 10);
                var approval25Reached = new Achievement(User, "GOALAPPROVALGEN25", "25 Benachrichtigung bestätigt", "Vielen Dank das du " + nameof(GoalTracker) + " nutzt! Wow du hast schon 25 Benachrichtigungen bestätigt.", 25);
                var approval50Reached = new Achievement(User, "GOALAPPROVALGEN50", "50 Benachrichtigung bestätigt", "Vielen Dank das du " + nameof(GoalTracker) + " nutzt! Unglaublich du hast schon 50 Benachrichtigungen bestätigt.", 50);

                var newAchievements = new List<Achievement>
                {
                    signUpAchievement, firstGoalCreatedAchievement, firstGoalEditedAchievement, approval10Reached,
                    approval25Reached, approval50Reached, successApproval10Reached, successApproval25Reached,
                    successApproval50Reached
                };

                await achievementRepository.AddRangeAsync(newAchievements);

                await LoadAchievementsAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task LoadUserAsync()
        {
            try
            {
                User = await userRepository.GetUserAsync();

                if (User != null)
                    WelcomeMessage = GetRandomWelcomeMessage(User.Name);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task ChangeUsernameAsync(string name)
        {
            try
            {
                await userRepository.ChangeUsernameAsync(name);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private string GetRandomWelcomeMessage(string name)
        {
            var random = new Random().Next(0, 3);

            return random switch
            {
                0 => $"Na {name}, wie geht's? 😄",
                1 => $"Hey {name}, schöner Tag oder? 😍",
                2 => $"Schön das du wieder an deinen Zielen dran bist {name} 🎯",
                3 => $"Ich hoffe dir gefällt {nameof(GoalTracker)}, {name} ♥️",
                _ => $"Irgendwas ist schief gelaufen {name}, wenn was schief geht, starte die App neu 😰 "
            };
        }
    }
}