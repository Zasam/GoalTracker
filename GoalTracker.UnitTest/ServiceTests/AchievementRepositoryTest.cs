using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoalTracker.UnitTest.ServiceTests
{
    [TestClass]
    public class AchievementRepositoryTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public AchievementRepositoryTest()
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
            var achievementRepository = new AchievementRepository(context);

            Assert.IsNotNull(context);
            Assert.IsNotNull(achievementRepository);
        }

        [TestMethod]
        public async Task AddAchievementsTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");

            await userRepository.AddUserAsync(testUser);

            var achievements = new[]
            {
                new Achievement(testUser, "TAGONE", "TEST1", "SIMPLE DESCRIPTION", 10),
                new Achievement(testUser, "TAGTWO", "TEST2", "SIMPLE DESCRIPTION", 20),
                new Achievement(testUser, "TAGTHREE", "TEST3", "SIMPLE DESCRIPTION", 30),
                new Achievement(testUser, "TAGFOUR", "TEST4", "SIMPLE DESCRIPTION", 40),
                new Achievement(testUser, "TAGFIVE", "TEST5", "SIMPLE DESCRIPTION", 50)
            };

            await achievementRepository.AddRangeAsync(achievements);

            var savedAchievements = await achievementRepository.GetAllAsync();
            var sAchievements = savedAchievements as Achievement[] ?? savedAchievements.ToArray();

            Assert.IsNotNull(savedAchievements);
            Assert.IsTrue(sAchievements.Count() == 5);
            Assert.AreEqual(sAchievements[0], achievements[0]);
            Assert.AreEqual(sAchievements[1], achievements[1]);
            Assert.AreEqual(sAchievements[2], achievements[2]);
            Assert.AreEqual(sAchievements[3], achievements[3]);
            Assert.AreEqual(sAchievements[4], achievements[4]);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AddAchievementsWithUnregisteredUserThrowsExceptionTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);

            var testUser = new User("Testuser");
            var achievements = new[]
            {
                new Achievement(testUser, "TAGONE", "TEST1", "SIMPLE DESCRIPTION", 10),
                new Achievement(testUser, "TAGTWO", "TEST2", "SIMPLE DESCRIPTION", 20),
                new Achievement(testUser, "TAGTHREE", "TEST3", "SIMPLE DESCRIPTION", 30),
                new Achievement(testUser, "TAGFOUR", "TEST4", "SIMPLE DESCRIPTION", 40),
                new Achievement(testUser, "TAGFIVE", "TEST5", "SIMPLE DESCRIPTION", 50)
            };

            await achievementRepository.AddRangeAsync(achievements);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AddAchievementsWithDifferentUsersThrowsException()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);

            var testUser = new User("Testuser");
            var testUser2 = new User("Testuser2");
            var achievements = new[]
            {
                new Achievement(testUser, "TAGONE", "TEST1", "SIMPLE DESCRIPTION", 10),
                new Achievement(testUser2, "TAGTWO", "TEST2", "SIMPLE DESCRIPTION", 20),
                new Achievement(testUser2, "TAGTHREE", "TEST3", "SIMPLE DESCRIPTION", 30),
                new Achievement(testUser2, "TAGFOUR", "TEST4", "SIMPLE DESCRIPTION", 40),
                new Achievement(testUser2, "TAGFIVE", "TEST5", "SIMPLE DESCRIPTION", 50)
            };

            await achievementRepository.AddRangeAsync(achievements);
        }


        [TestMethod]
        public async Task GetAchievementByInternalTagTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var achievementRepository = new AchievementRepository(context);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");

            await userRepository.AddUserAsync(testUser);

            var achievements = new[]
            {
                new Achievement(testUser, "TAGONE", "TEST1", "SIMPLE DESCRIPTION", 10),
                new Achievement(testUser, "TAGTWO", "TEST2", "SIMPLE DESCRIPTION", 20),
                new Achievement(testUser, "TAGTHREE", "TEST3", "SIMPLE DESCRIPTION", 30),
                new Achievement(testUser, "TAGFOUR", "TEST4", "SIMPLE DESCRIPTION", 40),
                new Achievement(testUser, "TAGFIVE", "TEST5", "SIMPLE DESCRIPTION", 50)
            };

            await achievementRepository.AddRangeAsync(achievements);

            var savedAchievement = await achievementRepository.GetByInternalTag("TAGONE");

            Assert.IsNotNull(savedAchievement);
            Assert.AreEqual(1, savedAchievement.Id);
            Assert.AreEqual(savedAchievement.User, achievements[0].User);
            Assert.AreEqual(savedAchievement.Title, achievements[0].Title);
            Assert.AreEqual(savedAchievement.InternalTag, achievements[0].InternalTag);
            Assert.AreEqual(savedAchievement.Description, achievements[0].Description);
            Assert.AreEqual(savedAchievement.Experience, achievements[0].Experience);
            Assert.AreEqual(savedAchievement.Unlocked, achievements[0].Unlocked);
        }
    }
}