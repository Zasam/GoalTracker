using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoalTracker.UnitTest.ViewModelTests
{
    [TestClass]
    public class SettingViewModelTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public SettingViewModelTest()
            : base(new DbContextOptionsBuilder<GoalTrackerContext>()
                .UseSqlite(InMemoryConnectionManager.CreateInMemoryConnection())
                .Options)
        {
            inMemoryConnectionManager = new InMemoryConnectionManager(ContextOptions);
        }

        public void Dispose()
        {
            inMemoryConnectionManager.Dispose();
        }

        [TestMethod]
        public void SetupServiceTest()
        {
            using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            Assert.IsNotNull(settingViewModel);
        }

        [TestMethod]
        public async Task RegisterUserAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);
            settingViewModel.LoadUserAsyncCommand.Execute(null);

            Assert.IsNotNull(settingViewModel.User);
            Assert.AreEqual("Default", settingViewModel.User.Name);
        }

        [TestMethod]
        public async Task ChangeUsernameAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);

            settingViewModel.LoadUserAsyncCommand.Execute(null);
            settingViewModel.ChangeUsernameAsyncCommand.Execute("Max");

            Assert.IsNotNull(settingViewModel.User);
            Assert.AreEqual("Max", settingViewModel.User.Name);
        }

        [TestMethod]
        public async Task CreateAchievementsAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);
            settingViewModel.CreateAchievementsAsyncCommand.Execute(null);
            Assert.IsNotNull(settingViewModel.Achievements);
            Assert.IsTrue(settingViewModel.Achievements.Any());

            var expectedProgressPoints = 0;

            foreach (var achievement in settingViewModel.Achievements.Where(a => a.Unlocked))
                expectedProgressPoints += achievement.Experience;

            Assert.AreEqual(expectedProgressPoints, settingViewModel.AchievementProgressPoints);

            var unlockedCount = settingViewModel.Achievements.Count(a => a.Unlocked);
            var generalCount = settingViewModel.Achievements.Count;
            var expectedProgress = unlockedCount / generalCount * generalCount;
            Assert.AreEqual(expectedProgress, settingViewModel.AchievementProgress);
        }

        [TestMethod]
        public async Task LoadAchievementAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);
            settingViewModel.CreateAchievementsAsyncCommand.Execute(null);
            Assert.IsNotNull(settingViewModel.Achievements);
            Assert.IsTrue(settingViewModel.Achievements.Any());

            settingViewModel.LoadAchievementAsyncCommand.Execute("SIGNUP");
            Assert.IsNotNull(settingViewModel.LoadedAchievement);
            Assert.AreEqual("SIGNUP", settingViewModel.LoadedAchievement.InternalTag);
        }

        [TestMethod]
        public async Task UnlockAchievementAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);
            settingViewModel.CreateAchievementsAsyncCommand.Execute(null);
            Assert.IsNotNull(settingViewModel.Achievements);
            Assert.IsTrue(settingViewModel.Achievements.Any());

            settingViewModel.UnlockAchievementAsyncCommand.Execute("SIGNUP");

            Assert.IsNotNull(settingViewModel.LoadedAchievement);
            Assert.AreEqual("SIGNUP", settingViewModel.LoadedAchievement.InternalTag);
            Assert.AreEqual(true, settingViewModel.LoadedAchievement.Unlocked);
        }

        [TestMethod]
        public async Task CreateAchievementsAsyncCommandWithUnregistedUserRegistersDefaultUserTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            Assert.IsNull(settingViewModel.User);
            settingViewModel.CreateAchievementsAsyncCommand.Execute(null);
            Assert.IsNotNull(settingViewModel.User);
            Assert.IsNotNull(settingViewModel.Achievements);
            Assert.IsTrue(settingViewModel.Achievements.Any());
        }
    }
}