﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class SettingViewModel : BaseViewModel, ISettingViewModel
    {
        #region Properties

        #region Repositories

        private readonly IAchievementRepository achievementRepository;
        private readonly IUserRepository userRepository;

        #endregion // Repositories

        private double achievementProgress;
        private int achievementProgressPoints;
        private List<Achievement> achievements;

        public string Username { get; set; }

        public List<Achievement> Achievements
        {
            get => achievements;
            set
            {
                achievements = value;
                OnPropertyChanged();
            }
        }

        public double AchievementProgress
        {
            get => achievementProgress;
            set
            {
                achievementProgress = value;
                OnPropertyChanged();
            }
        }

        public int AchievementProgressPoints
        {
            get => achievementProgressPoints;
            set
            {
                achievementProgressPoints = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenLinkCommand => new Command<string>(OpenLink);

        #endregion // Properties

        public SettingViewModel(IAchievementRepository achievementRepository, IUserRepository userRepository)
        {
            this.achievementRepository = achievementRepository;
            this.userRepository = userRepository;
        }

        private void OpenLink(string url)
        {
            Launcher.OpenAsync(url);
        }

        public async Task<Achievement> GetAchievementAsync(string internalTag)
        {
            try
            {
                var unlockableAchievements = await
                    achievementRepository.FindAsync(a => a.InternalTag == internalTag);
                var unlockableAchievement = unlockableAchievements.FirstOrDefault();
                return unlockableAchievement;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<bool> UnlockAchievementAsync(string internalTag)
        {
            try
            {
                var achievement = await GetAchievementAsync(internalTag);
                if (achievement != null)
                {
                    var firstUnlock = achievement.Unlock();

                    if (firstUnlock)
                    {
                        await achievementRepository.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return false;
        }

        public async Task LoadAchievementsAsync()
        {
            try
            {
                var achievementCollection = await achievementRepository.GetAllAsync();
                Achievements = achievementCollection.ToList();

                double unlockedAchievementsCount = Achievements.Count(a => a.Unlocked);
                double achievementsCount = Achievements.Count();
                AchievementProgress = Math.Round(unlockedAchievementsCount / achievementsCount * 100, 0);
                AchievementProgressPoints = Achievements.Where(a => a.Unlocked).Sum(a => a.Experience);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async IAsyncEnumerable<Tuple<string, int>> CreateDefaultUserAsync()
        {
            yield return new Tuple<string, int>("Datenbank wird initialisiert...", 33);
            var user = new User("Default");
            await userRepository.AddAsync(user);

            yield return new Tuple<string, int>("Erfolge werden erstellt...", 66);
            await CreateAchievementsAsync(user);

            yield return new Tuple<string, int>("Daten werden überprüft...", 100);
        }

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

                var newAchievements = new List<Achievement>
                {
                    signupAchievement, firstGoalCreatedAchievement, firstGoalEditedAchievement, approval10Reached,
                    approval25Reached, approval50Reached, successApproval10Reached, successApproval25Reached,
                    successApproval50Reached
                };

                await achievementRepository.AddRangeAsync(newAchievements);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        public async void LoadUsername()
        {
            Username = await userRepository.GetUsernameAsync();
        }
    }
}