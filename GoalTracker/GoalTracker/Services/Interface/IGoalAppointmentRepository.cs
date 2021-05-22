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
        Task<IEnumerable<GoalAppointment>> GetAllByApprovalDayAsync(DateTime day);
        Task<GoalAppointment> GetByParentAndAppointmentDateAsync(Goal goal, DateTime? date);
        Task<GoalAppointment> GetByParentAndApprovalDayAsync(Goal goal, DateTime date);
        Task AddRangeAsync(IEnumerable<GoalAppointment> goalAppointments);
    }
}