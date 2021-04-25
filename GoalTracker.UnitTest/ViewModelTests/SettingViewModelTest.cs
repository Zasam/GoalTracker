using System;
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
        public async Task RegisterUserTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            var newUser = await settingViewModel.RegisterDefaultUserAsync();
            Assert.IsNotNull(newUser);
            Assert.AreEqual(newUser.Id, 1);
            Assert.AreEqual("Default", newUser.Name);
            Assert.IsNotNull(newUser.Achievements);
        }

        [TestMethod]
        public async Task CreateAchievementsTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);

            var newUser = await settingViewModel.RegisterDefaultUserAsync();
            await settingViewModel.CreateAchievementsAsync(newUser);
            await settingViewModel.LoadAchievementsAsync();

            Assert.IsNotNull(settingViewModel.Achievements);
            Assert.AreEqual(9, settingViewModel.Achievements.Count);
        }
    }
}