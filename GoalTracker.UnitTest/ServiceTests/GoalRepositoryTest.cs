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
    public class GoalRepositoryTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalRepositoryTest()
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
            var goalRepository = new GoalRepository(context);

            Assert.IsNotNull(context);
            Assert.IsNotNull(goalRepository);
        }

        [TestMethod]
        public async Task AddGoalAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var startDate = DateTime.Now;

            var testGoal = new Goal("Testgoal", "Testnotes", startDate, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);
            var savedGoal = await goalRepository.GetByTitleAsync("Testgoal");

            Assert.IsNotNull(savedGoal);
            Assert.AreEqual(1, savedGoal.Id);
            Assert.AreEqual("Testgoal", savedGoal.Title);
            Assert.AreEqual("Testnotes", savedGoal.Notes);
            Assert.AreEqual(startDate, savedGoal.StartDate);
            Assert.IsFalse(savedGoal.HasDueDate);
            Assert.AreEqual(DateTime.MaxValue, savedGoal.EndDate);
            Assert.AreEqual(GoalAppointmentInterval.Halbstündlich, savedGoal.GoalAppointmentInterval);
            Assert.AreEqual(TimeSpan.FromDays(1), savedGoal.NotificationTime);
            Assert.AreEqual("source.png", savedGoal.DetailImage);
            Assert.IsFalse(savedGoal.AllTasksCompleted);
            Assert.AreEqual(0, savedGoal.GoalTaskCount);
            Assert.IsNotNull(savedGoal.GoalAppointments);
            Assert.IsNotNull(savedGoal.GoalTasks);
        }

        [TestMethod]
        public async Task GetAllAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var startDate = DateTime.Now;

            var testGoal = new Goal("Testgoal", "Testnotes", startDate, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            var testGoal2 = new Goal("Testgoal2", "Testnotes", startDate, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);
            await goalRepository.AddAsync(testGoal2);
            var goals = await goalRepository.GetAllAsync();

            Assert.IsNotNull(goals);
            Assert.AreEqual(2, goals.Count());
        }
    }
}