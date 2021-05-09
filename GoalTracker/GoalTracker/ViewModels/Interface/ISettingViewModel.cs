using System.Collections.Generic;
using System.Windows.Input;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface ISettingViewModel
    {
        List<Achievement> Achievements { get; set; }
        double AchievementProgress { get; set; }
        int AchievementProgressPoints { get; set; }
        User User { get; set; }
        public string Username { get; set; }

        public string WelcomeMessage { get; set; }
        Achievement LoadedAchievement { get; set; }

        ICommand OpenLinkCommand { get; }
        ICommand LoadAchievementAsyncCommand { get; }
        ICommand UnlockAchievementAsyncCommand { get; }
        ICommand LoadAchievementsAsyncCommand { get; }
        ICommand RegisterDefaultUserAsyncCommand { get; }
        ICommand CreateAchievementsAsyncCommand { get; }
        ICommand LoadUserAsyncCommand { get; }
        ICommand ChangeUsernameAsyncCommand { get; }
    }
}