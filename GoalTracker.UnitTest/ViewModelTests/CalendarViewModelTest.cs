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
    public class CalendarViewModelTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public CalendarViewModelTest()
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
            var calendarViewModel = new CalendarViewModel(goalRepository, goalAppointmentRepository);

            Assert.IsNotNull(calendarViewModel);
        }

        [TestMethod]
        public async Task LoadCalendarEventsWithApprovalsAsyncCommandTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalRepository = new GoalRepository(context);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var calendarViewModel = new CalendarViewModel(goalRepository, goalAppointmentRepository);

            var testGoal = new Goal("Testgoal", "Notes", DateTime.Today, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "Source.png");
            await goalRepository.AddAsync(testGoal);

            var goalAppointmentOne = new GoalAppointment(testGoal, DateTime.Today);
            goalAppointmentOne.Approve(true);
            var goalAppointmentTwo = new GoalAppointment(testGoal, DateTime.Today.AddDays(1));
            goalAppointmentTwo.Approve(false);
            var goalAppointmentThree = new GoalAppointment(testGoal, DateTime.Today.AddDays(2));
            var goalAppointmentFour = new GoalAppointment(testGoal, DateTime.Today.AddDays(3));
            var goalAppointments = new List<GoalAppointment> {goalAppointmentOne, goalAppointmentTwo, goalAppointmentThree, goalAppointmentFour};

            await goalAppointmentRepository.AddRangeAsync(goalAppointments);

            calendarViewModel.LoadEventsAsyncCommand.Execute(null);

            Assert.IsNotNull(calendarViewModel.CalendarInlineEvents);
            Assert.AreEqual(4, calendarViewModel.CalendarInlineEvents.Count);
            Assert.AreEqual(1, calendarViewModel.SuccessApprovalsWeek);
            Assert.AreEqual(1, calendarViewModel.FailureApprovalsWeek);
        }
    }
}