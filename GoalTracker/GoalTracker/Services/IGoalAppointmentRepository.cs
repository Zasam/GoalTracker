using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public interface IGoalAppointmentRepository : IRepository<GoalAppointment>
    {
        public new Task<IEnumerable<GoalAppointment>> GetAllAsync();
        public Task<IEnumerable<GoalAppointment>> GetAllByParentAsync(Goal goal);
        public Task<IEnumerable<GoalAppointment>> GetAllByDayAsync(DateTime day);
        public Task<GoalAppointment> GetByParentAndDayAsync(Goal goal, DateTime date);
    }
}