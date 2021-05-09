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
    public class GoalTaskRepositoryTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalTaskRepositoryTest()
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
            var goalTaskRepository = new GoalTaskRepository(context);

            Assert.IsNotNull(context);
            Assert.IsNotNull(goalTaskRepository);
        }

        [TestMethod]
        public async Task AddGoalTaskAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            await goalRepository.AddAsync(parent);

            var goalTask = new GoalTask(parent, "Task", "", false);
            await goalTaskRepository.AddAsync(goalTask);
            var goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.IsTrue(goalTasks.Any());

            goalTask = goalTasks.FirstOrDefault();

            Assert.IsNotNull(goalTasks);
            Assert.AreEqual("Task", goalTask.Title);
            Assert.AreEqual("", goalTask.Notes);
            Assert.IsFalse(goalTask.Completed);
        }


        [TestMethod]
        public async Task AddGoalTasksAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            await goalRepository.AddAsync(parent);

            var goalTask1 = new GoalTask(parent, "TaskOne", "Notes1", false);
            var goalTask2 = new GoalTask(parent, "TaskTwo", "Notes2", true);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTask1, goalTask2});
            var goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.AreEqual(2, goalTasks.Count());

            var goalTasksSorted = goalTasks.ToArray();

            Assert.IsNotNull(goalTasks);

            Assert.AreEqual("TaskOne", goalTasksSorted[0].Title);
            Assert.AreEqual("Notes1", goalTasksSorted[0].Notes);
            Assert.IsFalse(goalTasksSorted[0].Completed);

            Assert.AreEqual("TaskTwo", goalTasksSorted[1].Title);
            Assert.AreEqual("Notes2", goalTasksSorted[1].Notes);
            Assert.IsTrue(goalTasksSorted[1].Completed);
        }

        [TestMethod]
        public async Task GetAllByParentAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            var parentTwo = new Goal("Testgoal2", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);
            await goalRepository.AddAsync(parentTwo);

            var goalTask1 = new GoalTask(parent, "TaskOne", "Notes1", false);
            var goalTask2 = new GoalTask(parent, "TaskTwo", "Notes2", true);
            var goalTask3 = new GoalTask(parentTwo, "TaskOne", "Notes", false);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTask1, goalTask2, goalTask3});
            var p1GoalTasks = await goalTaskRepository.GetAllByParentAsync(parent);
            var p2GoalTasks = await goalTaskRepository.GetAllByParentAsync(parentTwo);

            Assert.IsNotNull(p1GoalTasks);
            Assert.IsNotNull(p2GoalTasks);

            Assert.AreEqual(2, p1GoalTasks.Count());
            Assert.AreEqual(1, p2GoalTasks.Count());
        }

        [TestMethod]
        public async Task RemoveGoalTaskAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);

            var goalTask = new GoalTask(parent, "TaskOne", "Notes1", false);
            await goalTaskRepository.AddAsync(goalTask);
            var goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.IsTrue(goalTasks.Any());

            await goalTaskRepository.RemoveAsync(goalTasks.FirstOrDefault());

            goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.IsFalse(goalTasks.Any());
        }

        [TestMethod]
        public async Task RemoveGoalTasksAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);

            var goalTaskOne = new GoalTask(parent, "TaskOne", "Notes", false);
            var goalTaskTwo = new GoalTask(parent, "TaskTwo", "Notes1", false);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});
            var goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.IsTrue(goalTasks.Any());

            await goalTaskRepository.RemoveRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});

            goalTasks = await goalTaskRepository.GetAllByParentAsync(parent);

            Assert.IsNotNull(goalTasks);
            Assert.IsFalse(goalTasks.Any());
        }
    }
}