using System;
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
    public class GoalViewModelTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalViewModelTest()
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
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);

            Assert.IsNotNull(goalViewModel);
        }

        [TestMethod]
        public async Task AddGoalAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel)
            {
                GoalTitle = "Testgoal",
                GoalNotes = "Notes",
                GoalStartDate = DateTime.Today,
                GoalHasDueDate = false,
                GoalEndDate = DateTime.MaxValue,
                GoalNotificationIntervalIndex = (int) GoalAppointmentInterval.Täglich,
                GoalNotificationTime = TimeSpan.FromHours(8),
                GoalImage = "test.png"
            };

            Assert.IsNotNull(goalViewModel.BindedGoal);
            Assert.AreEqual("Testgoal", goalViewModel.BindedGoal.Title);
            Assert.AreEqual("Notes", goalViewModel.BindedGoal.Notes);
            Assert.AreEqual(DateTime.Today, goalViewModel.BindedGoal.StartDate);
            Assert.AreEqual(false, goalViewModel.BindedGoal.HasDueDate);
            Assert.AreEqual(DateTime.MaxValue, goalViewModel.BindedGoal.EndDate);
            Assert.AreEqual(GoalAppointmentInterval.Täglich, goalViewModel.BindedGoal.GoalAppointmentInterval);
            Assert.AreEqual(TimeSpan.FromHours(8), goalViewModel.BindedGoal.NotificationTime);
            Assert.AreEqual("test.png", goalViewModel.BindedGoal.DetailImage);

            goalViewModel.AddGoalAsyncCommand.Execute(goalViewModel.BindedGoal);

            Assert.IsNotNull(goalViewModel.BindedGoal);

            //TODO: 
            //var goalTaskOne = new GoalTask(goalViewModel.BindedGoal, "TaskOne", "Notes", false);
            //var goalTaskTwo = new GoalTask(goalViewModel.BindedGoal, "TaskTwo", "Notes", true);

            //await goalTaskRepository.AddRangeAsync(new List<GoalTask> {goalTaskOne, goalTaskTwo});

            //var goalAppointments = await goalAppointmentRepository.GetAllByParentAsync(goalViewModel.BindedGoal);

            //// appointments are automatically added when executing the AddGoalAsyncCommand, for each interval, specified between start and end date (or start date +1 Month, when no end date is specified) are generated
            //Assert.IsNotNull(goalAppointments);
            ////TODO: Check for weeks in month? Which amount of appointments is expected?
            //Assert.AreEqual(5, goalAppointments.Count());

            //Assert.IsNotNull(goalViewModel.BindedGoal.GoalAppointments);
            //Assert.AreEqual(5, goalViewModel.BindedGoal.GoalAppointments.Count());

            //var goalTasks = await goalTaskRepository.GetAllByParentAsync(goalViewModel.BindedGoal);

            //Assert.IsNotNull(goalTasks);
            //Assert.AreEqual(2, goalTasks.Count());

            //Assert.IsNotNull(goalViewModel.BindedGoal.GoalTasks);
            //Assert.AreEqual(2, goalViewModel.BindedGoal.GoalTasks.Count());
        }

        //TODO: Add tests for loading of goals and properly assignment of all properties
    }
}