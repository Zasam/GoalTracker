using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface ISettingViewModel
    {
        List<Achievement> Achievements { get; }
        double AchievementProgress { get; }
        int AchievementProgressPoints { get; }
        User User { get; }
        string Username { get; set; }
        ICommand OpenLinkCommand { get; }
        Task<Achievement> GetAchievementAsync(string internalTag);
        Task<bool> UnlockAchievementAsync(string internalTag);
        Task LoadAchievementsAsync();
        Task<User> RegisterDefaultUserAsync();
        Task CreateAchievementsAsync(User associatedUser);
        Task LoadUserAsync();
        Task ChangeUsername(string name);
    }
}