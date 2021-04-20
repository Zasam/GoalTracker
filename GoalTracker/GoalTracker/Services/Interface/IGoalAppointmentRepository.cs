using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services.Interface
{
    public interface IGoalAppointmentRepository : IRepository<GoalAppointment>
    {
        Task<GoalAppointment> GetByIdAsync(int id);
        Task<IEnumerable<GoalAppointment>> GetAllAsync();
        Task<IEnumerable<GoalAppointment>> GetAllByParentAsync(Goal goal);
        Task<IEnumerable<GoalAppointment>> GetAllByDayAsync(DateTime day);
        Task<GoalAppointment> GetByParentAndDayAsync(Goal goal, DateTime date);
        Task AddRangeAsync(IEnumerable<GoalAppointment> goalAppointments);
    }
}