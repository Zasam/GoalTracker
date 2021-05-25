using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;
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
            try
            {
                return await GetAsync(id);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public new async Task<IEnumerable<GoalAppointment>> GetAllAsync()
        {
            try
            {
                return await context.GoalAppointments.Include(ga => ga.Goal).ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<GoalAppointment>> GetAllByParentAsync(Goal goal)
        {
            try
            {
                return await FindAsync(ga => ga.GoalId == goal.Id);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<GoalAppointment>> GetAllByApprovalDayAsync(DateTime day)
        {
            try
            {
                return await FindAsync(ga => ga.ApprovalDate.HasValue && ga.ApprovalDate.Value.Day == day.Day && ga.ApprovalDate.Value.Month == day.Month && ga.ApprovalDate.Value.Year == day.Year);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<GoalAppointment> GetByParentAndAppointmentDateAsync(Goal goal, DateTime? date)
        {
            try
            {
                date ??= DateTime.Now;
                var goalAppointments = await FindAsync(ga => ga.GoalId == goal.Id && ga.AppointmentDate <= date.Value);
                var goalAppointmentWithClosestDate = goalAppointments.Aggregate((ga1, ga2) => ga1.AppointmentDate > ga2.AppointmentDate ? ga1 : ga2);
                return goalAppointmentWithClosestDate;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<GoalAppointment> GetByParentAndApprovalDayAsync(Goal goal, DateTime day)
        {
            try
            {
                var goalAppointments = await FindAsync(ga =>
                    ga.GoalId == goal.Id && ga.ApprovalDate.HasValue && ga.ApprovalDate.Value.Day == day.Day && ga.ApprovalDate.Value.Month == day.Month && ga.ApprovalDate.Value.Year == day.Year);
                return goalAppointments.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public new async Task AddRangeAsync(IEnumerable<GoalAppointment> goalAppointments)
        {
            try
            {
                await base.AddRangeAsync(goalAppointments);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}