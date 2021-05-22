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
    public class GoalAppointmentRepositoryTest : BusinessLogicTest, IDisposable
    {
        private readonly InMemoryConnectionManager inMemoryConnectionManager;

        public GoalAppointmentRepositoryTest()
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
            var goalTaskRepository = new GoalAppointmentRepository(context);

            Assert.IsNotNull(context);
            Assert.IsNotNull(goalTaskRepository);
        }

        [TestMethod]
        public async Task GetAllByParentAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            var parentTwo = new Goal("Testgoal2", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);
            await goalRepository.AddAsync(parentTwo);

            var appointmentOne = new GoalAppointment(parent, DateTime.Today);
            var appointmentTwo = new GoalAppointment(parent, DateTime.Today.AddDays(1));
            var appointmentThree = new GoalAppointment(parentTwo, DateTime.Today.AddDays(2));

            await goalAppointmentRepository.AddRangeAsync(new List<GoalAppointment> {appointmentOne, appointmentTwo, appointmentThree});

            var appointmentsP1 = await goalAppointmentRepository.GetAllByParentAsync(parent);
            Assert.IsNotNull(appointmentsP1);
            Assert.AreEqual(2, appointmentsP1.Count());

            var appointmentsP2 = await goalAppointmentRepository.GetAllByParentAsync(parentTwo);
            Assert.IsNotNull(appointmentsP2);
            Assert.AreEqual(1, appointmentsP2.Count());
        }

        [TestMethod]
        public async Task GetAllByApprovalDayAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            var parentTwo = new Goal("Testgoal2", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);
            await goalRepository.AddAsync(parentTwo);

            var appointmentOne = new GoalAppointment(parent, DateTime.Today);
            appointmentOne.Approve(true);
            var appointmentTwo = new GoalAppointment(parent, DateTime.Today.AddDays(1));
            var appointmentThree = new GoalAppointment(parentTwo, DateTime.Today.AddDays(2));
            appointmentThree.Approve(true);
            var appointmentFour = new GoalAppointment(parentTwo, DateTime.Today);

            await goalAppointmentRepository.AddRangeAsync(new List<GoalAppointment> {appointmentOne, appointmentTwo, appointmentThree, appointmentFour});

            var appointments = await goalAppointmentRepository.GetAllByApprovalDayAsync(DateTime.Now);

            Assert.IsNotNull(appointments);
            Assert.AreEqual(2, appointments.Count());
        }

        [TestMethod]
        public async Task GetAllByParentAndApprovalDayAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");
            var parentTwo = new Goal("Testgoal2", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);
            await goalRepository.AddAsync(parentTwo);

            var appointmentOne = new GoalAppointment(parent, DateTime.Today);
            appointmentOne.Approve(true);
            var appointmentTwo = new GoalAppointment(parent, DateTime.Today.AddDays(1));
            var appointmentThree = new GoalAppointment(parentTwo, DateTime.Today.AddDays(2));
            appointmentThree.Approve(true);
            var appointmentFour = new GoalAppointment(parentTwo, DateTime.Today);

            await goalAppointmentRepository.AddRangeAsync(new List<GoalAppointment> {appointmentOne, appointmentTwo, appointmentThree, appointmentFour});

            //TODO: Rethink this test method! why only one entity returned here?
            var appointment = await goalAppointmentRepository.GetByParentAndApprovalDayAsync(parent, DateTime.Now);

            Assert.IsNotNull(appointment);
        }

        [TestMethod]
        public async Task GetByIdAsyncTest()
        {
            await using var context = new GoalTrackerContext(ContextOptions);
            var goalAppointmentRepository = new GoalAppointmentRepository(context);
            var goalRepository = new GoalRepository(context);

            var parent = new Goal("Testgoal", "", DateTime.Now, false, DateTime.MaxValue, GoalAppointmentInterval.Halbstündlich, TimeSpan.FromDays(1), 0, 0, "source.png");

            await goalRepository.AddAsync(parent);

            var appointmentOne = new GoalAppointment(parent, DateTime.Today);
            var appointmentTwo = new GoalAppointment(parent, DateTime.Today.AddDays(1));

            await goalAppointmentRepository.AddRangeAsync(new List<GoalAppointment> {appointmentOne, appointmentTwo});

            var appointment = await goalAppointmentRepository.GetByIdAsync(1);

            Assert.IsNotNull(appointment);
            Assert.AreEqual(1, appointment.Id);
            Assert.AreEqual(DateTime.Today, appointment.AppointmentDate);
        }
    }
}