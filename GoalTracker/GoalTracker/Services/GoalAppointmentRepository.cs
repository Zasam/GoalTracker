using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Services
{
    public class GoalAppointmentRepository : Repository<GoalAppointment>, IGoalAppointmentRepository
    {
        private readonly IGoalTrackerContext context;

        public GoalAppointmentRepository(IGoalTrackerContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<GoalAppointment> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public new async Task<IEnumerable<GoalAppointment>> GetAllAsync()
        {
            return await context.GoalAppointments.Include(ga => ga.Goal).ToListAsync();
        }

        public async Task<IEnumerable<GoalAppointment>> GetAllByParentAsync(Goal goal)
        {
            return await FindAsync(ga => ga.GoalId == goal.Id);
        }

        public async Task<IEnumerable<GoalAppointment>> GetAllByDayAsync(DateTime day)
        {
            return await FindAsync(ga => ga.ApprovalDate.HasValue && ga.ApprovalDate.Value == day.Date);
        }

        public async Task<GoalAppointment> GetByParentAndDayAsync(Goal goal, DateTime day)
        {
            var goalAppointments = await FindAsync(ga =>
                ga.GoalId == goal.Id && ga.ApprovalDate.HasValue && ga.ApprovalDate == day.Date);
            return goalAppointments.FirstOrDefault();
        }

        public new async Task AddRangeAsync(IEnumerable<GoalAppointment> goalAppointments)
        {
            await base.AddRangeAsync(goalAppointments);
        }
    }
}