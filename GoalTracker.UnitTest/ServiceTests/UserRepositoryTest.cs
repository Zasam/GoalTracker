using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoalTracker.UnitTest.ServiceTests
{
    [TestClass]
    public class UserRepositoryTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public UserRepositoryTest()
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

            Assert.IsNotNull(context);
            Assert.IsNotNull(userRepository);
        }

        [TestMethod]
        public async Task AddUserAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");
            await userRepository.AddUserAsync(testUser);
            var savedTestUser = await userRepository.GetUserAsync();

            Assert.IsNotNull(savedTestUser);
            Assert.AreEqual(1, savedTestUser.Id);
            Assert.AreEqual("Testuser", savedTestUser.Name);
        }

        [TestMethod]
        public async Task ChangeUsernameAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");
            await userRepository.AddUserAsync(testUser);
            testUser = await userRepository.GetUserAsync();
            await userRepository.ChangeUsernameAsync("Testuser2");
            var savedTestUser = await userRepository.GetUserAsync();

            Assert.IsNotNull(savedTestUser);
            Assert.AreEqual("Testuser2", savedTestUser.Name);
            Assert.AreEqual(testUser.Id, savedTestUser.Id);
        }

        [TestMethod]
        public async Task GetRegisteredUserTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");
            await userRepository.AddUserAsync(testUser);
            testUser = await userRepository.GetUserAsync();

            Assert.IsNotNull(testUser);
            Assert.IsNotNull(testUser.Achievements);
            Assert.IsInstanceOfType(testUser, typeof(User));
            Assert.IsInstanceOfType(testUser.Achievements, typeof(IEnumerable<Achievement>));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task RegisterTwoUsersThrowsExceptionTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);

            var testUser = new User("Testuser");
            var testUser2 = new User("Testuser2");
            await userRepository.AddUserAsync(testUser);
            await userRepository.AddUserAsync(testUser2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ChangeUsernameAsyncWithNoRegisteredUserThrowsExceptionTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var userRepository = new UserRepository(context);
            await userRepository.ChangeUsernameAsync("Test");
        }
    }
}