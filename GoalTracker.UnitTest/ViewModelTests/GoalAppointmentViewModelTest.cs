using System;
using System.Linq;
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
    public class GoalAppointmentViewModelTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalAppointmentViewModelTest()
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
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var parent = new Goal();
            var goalAppointmentViewModel = new GoalAppointmentViewModel(parent, goalAppointmentRepository);
            Assert.IsNotNull(goalAppointmentViewModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetupServiceWithEmptyParentThrowsExceptionTest()
        {
            using var context = new GoalTrackerContext(ContextOptions);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalAppointmentViewModel = new GoalAppointmentViewModel(null, goalAppointmentRepository);
        }

        [TestMethod]
        public async Task LoadGoalAppointmentsAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var parent = new Goal("Testgoal", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Wöchentlich, TimeSpan.FromDays(1), "source.png");
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);
            goalViewModel.AddGoalAsyncCommand.Execute(parent);

            var goalAppointmentViewModel = new GoalAppointmentViewModel(parent, goalAppointmentRepository);
            goalAppointmentViewModel.LoadAppointmentsAsyncCommand.Execute(null);

            Assert.IsNotNull(goalAppointmentViewModel.GoalAppointments);
            Assert.AreEqual(5, goalAppointmentViewModel.GoalAppointments.Length);
        }

        [TestMethod]
        public async Task ApproveGoalAppointmentAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalTaskRepository = new GoalTaskRepository(context);
            var userRepository = new UserRepository(context);
            var achievementRepository = new AchievementRepository(context);
            var settingViewModel = new SettingViewModel(achievementRepository, userRepository);
            var parent = new Goal("Testgoal", "Testnotes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Wöchentlich, TimeSpan.FromDays(1), "source.png");
            var goalViewModel = new GoalViewModel(goalRepository, goalAppointmentRepository, goalTaskRepository, userRepository, settingViewModel);
            goalViewModel.AddGoalAsyncCommand.Execute(parent);

            var goalAppointmentViewModel = new GoalAppointmentViewModel(parent, goalAppointmentRepository);
            goalAppointmentViewModel.LoadAppointmentsAsyncCommand.Execute(null);
            var appointment = goalAppointmentViewModel.GoalAppointments.FirstOrDefault();
            goalAppointmentViewModel.ApproveAppointmentAsyncCommand.Execute(appointment);

            Assert.IsNotNull(goalAppointmentViewModel.SelectedGoalAppointment);
            Assert.IsTrue(goalAppointmentViewModel.SelectedGoalAppointment.Success);
            Assert.IsTrue(goalAppointmentViewModel.GoalAppointments[0].Success);
        }
    }
}