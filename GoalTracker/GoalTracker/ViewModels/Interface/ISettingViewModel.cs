using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface ISettingViewModel
    {
        List<Achievement> Achievements { get; set; }
        double AchievementProgress { get; set; }
        int AchievementProgressPoints { get; set; }
        string Username { get; set; }
        Task<Achievement> GetAchievementAsync(string internalTag);
        Task<bool> UnlockAchievementAsync(string internalTag);
        Task LoadAchievementsAsync();
        IAsyncEnumerable<Tuple<string, int>> CreateDefaultUserAsync();
        void LoadUsername();
    }
}