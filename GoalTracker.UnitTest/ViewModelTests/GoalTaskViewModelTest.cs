using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoalTracker.UnitTest.ViewModelTests
{
    [TestClass]
    public class GoalTaskViewModelTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalTaskViewModelTest()
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
            var parent = new Goal();
            var goalTaskViewModel = new GoalTaskViewModel(parent, goalTaskRepository);
            Assert.IsNotNull(goalTaskViewModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetupServiceWithEmptyParentThrowsExceptionTest()
        {
            using var context = new GoalTrackerContext(ContextOptions);
            var goalTaskRepository = new GoalTaskRepository(context);
            var goalTaskViewModel = new GoalTaskViewModel(null, goalTaskRepository);
        }

        [TestMethod]
        public async Task LoadGoalTasksAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);

            var parent = new Goal("Testgoal", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Wöchentlich, TimeSpan.FromDays(1), "source.png");

            goalViewModel.AddGoalAsyncCommand.Execute(parent);

            var goalTaskOne = new GoalTask(parent, "TaskOne", "Notes", false);
            var goalTaskTwo = new GoalTask(parent, "TaskTwo", "Notes", true);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});

            var goalTaskViewModel = new GoalTaskViewModel(parent, goalTaskRepository);
            goalTaskViewModel.LoadTasksAsyncCommand.Execute(null);

            Assert.IsNotNull(goalTaskViewModel.GoalTasks);
            Assert.AreEqual(2, goalTaskViewModel.GoalTasks.Length);
        }

        [TestMethod]
        public async Task SetTaskCompletedAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);

            var parent = new Goal("Testgoal", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Wöchentlich, TimeSpan.FromDays(1), "source.png");

            goalViewModel.AddGoalAsyncCommand.Execute(parent);

            var goalTaskOne = new GoalTask(parent, "TaskOne", "Notes", false);
            var goalTaskTwo = new GoalTask(parent, "TaskTwo", "Notes", true);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});

            var goalTaskViewModel = new GoalTaskViewModel(parent, goalTaskRepository);
            var testTask = goalTaskViewModel.GoalTasks[0];
            goalTaskViewModel.SetTaskCompletedAsyncCommand.Execute(testTask);

            Assert.IsNotNull(goalTaskViewModel.SelectedGoalTask);
            Assert.AreEqual(testTask, goalTaskViewModel.SelectedGoalTask);
            Assert.IsTrue(goalTaskViewModel.SelectedGoalTask.Completed);
            Assert.IsTrue(goalTaskViewModel.GoalTasks[0].Completed);

            goalTaskViewModel.SetTaskCompletedAsyncCommand.Execute(testTask);

            Assert.IsNotNull(goalTaskViewModel.SelectedGoalTask);
            Assert.AreEqual(testTask, goalTaskViewModel.SelectedGoalTask);
            Assert.IsFalse(goalTaskViewModel.SelectedGoalTask.Completed);
            Assert.IsFalse(goalTaskViewModel.GoalTasks[0].Completed);
        }

        [TestMethod]
        public async Task DeleteTaskAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);

            var parent = new Goal("Testgoal", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Wöchentlich, TimeSpan.FromDays(1), "source.png");

            goalViewModel.AddGoalAsyncCommand.Execute(parent);

            var goalTaskOne = new GoalTask(parent, "TaskOne", "Notes", false);
            var goalTaskTwo = new GoalTask(parent, "TaskTwo", "Notes", true);
            await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});

            var goalTaskViewModel = new GoalTaskViewModel(parent, goalTaskRepository);
            var testTask = goalTaskViewModel.GoalTasks[0];
            goalTaskViewModel.DeleteTaskAsyncCommand.Execute(testTask);

            Assert.AreEqual(1, goalTaskViewModel.GoalTasks.Length);
            Assert.AreNotEqual(goalTaskViewModel.GoalTasks[0], testTask);
        }
    }
}