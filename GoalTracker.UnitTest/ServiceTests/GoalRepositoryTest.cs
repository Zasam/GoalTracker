using System;
using System.Collections.Generic;
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

        [TestMethod]
        public async Task GetAllStartedAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var testGoal = new Goal("Testgoal", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            var testGoal2 = new Goal("Testgoal2", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            var testGoal3 = new Goal("Testgoal3", "Testnotes", DateTime.Today.AddDays(1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");

            await goalRepository.AddAsync(testGoal);
            await goalRepository.AddAsync(testGoal2);
            await goalRepository.AddAsync(testGoal3);

            var startedGoals = await goalRepository.GetAllStartedAsync(DateTime.Today);

            Assert.IsNotNull(startedGoals);
            Assert.AreEqual(2, startedGoals.Count());
        }

        [TestMethod]
        public async Task GetByTitleAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var testGoal = new Goal("Testgoal", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);

            var savedGoal = await goalRepository.GetByTitleAsync("Testgoal");
            Assert.IsNotNull(savedGoal);
            Assert.AreEqual("Testgoal", savedGoal.Title);
        }

        [TestMethod]
        public async Task RemoveAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var testGoal = new Goal("Testgoal", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);

            var goals = await goalRepository.GetAllAsync();
            Assert.IsNotNull(goals);
            Assert.AreEqual(1, goals.Count());

            await goalRepository.RemoveAsync(goals.FirstOrDefault());

            goals = await goalRepository.GetAllAsync();
            Assert.IsNotNull(goals);
            Assert.AreEqual(0, goals.Count());
        }

        [TestMethod]
        public async Task RemoveRangeAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);

            var testGoal = new Goal("Testgoal", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            var testGoal2 = new Goal("Testgoal2", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);
            await goalRepository.AddAsync(testGoal2);

            var goals = await goalRepository.GetAllAsync();
            Assert.IsNotNull(goals);
            Assert.AreEqual(2, goals.Count());

            await goalRepository.RemoveRangeAsync(goals);

            goals = await goalRepository.GetAllAsync();
            Assert.IsNotNull(goals);
            Assert.AreEqual(0, goals.Count());
        }

        [TestMethod]
        public async Task UpdateAllTasksCompletedAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);

            var testGoal = new Goal("Testgoal", "Testnotes", DateTime.Today.AddDays(-1), false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), "source.png");
            await goalRepository.AddAsync(testGoal);

            var goalTask = new GoalTask(testGoal, "TaskOne", "Notes", true);
            var goalTask2 = new GoalTask(testGoal, "TaskTwo", "Notes", true);

            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTask, goalTask2});
            await goalRepository.UpdateAllTasksCompletedAsync(testGoal);
            var goal = await goalRepository.GetByTitleAsync("Testgoal");

            Assert.IsNotNull(goal);
            Assert.IsTrue(goal.AllTasksCompleted);
        }
    }
}