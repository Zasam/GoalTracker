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
    public class GoalRepository : Repository<Goal>, IGoalRepository
    {
        private readonly IGoalTrackerContext context;

        public GoalRepository(IGoalTrackerContext context)
            : base(context)
        {
            this.context = context;
        }

        public new async Task AddAsync(Goal goal)
        {
            await base.AddAsync(goal);
        }

        public new async Task<IEnumerable<Goal>> GetAllAsync()
        {
            return await context.Goals.Include(g => g.GoalAppointments).Include(g => g.GoalTasks).ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetAllInDateAsync(DateTime date)
        {
            return await FindAsync(g =>
                !g.HasDueDate && g.StartDate <= date || g.StartDate <= date && g.EndDate >= date);
        }

        public new async Task RemoveAsync(Goal goal)
        {
            var goalAppointments = context.GoalAppointments.Where(ga => ga.GoalId == goal.Id);
            var goalTasks = context.GoalTasks.Where(gt => gt.GoalId == goal.Id);
            context.GoalAppointments.RemoveRange(goalAppointments);
            context.GoalTasks.RemoveRange(goalTasks);
            await base.RemoveAsync(goal);
        }

        public new async Task RemoveRangeAsync(IEnumerable<Goal> goals)
        {
            var goalCollection = goals as Goal[] ?? goals.ToArray();
            foreach (var goal in goalCollection)
            {
                var goalAppointments = context.GoalAppointments.Where(ga => ga.GoalId == goal.Id);
                var goalTasks = context.GoalTasks.Where(gt => gt.GoalId == goal.Id);
                context.GoalAppointments.RemoveRange(goalAppointments);
                context.GoalTasks.RemoveRange(goalTasks);
            }

            await base.RemoveRangeAsync(goalCollection);
        }

        public async Task<Goal> GetByTitleAsync(string title)
        {
            var goals = await FindAsync(g => g.Title == title);
            return goals.FirstOrDefault();
        }

        public async Task UpdateAllTasksCompletedAsync(Goal goal)
        {
            var goalTasks = context.GoalTasks.Where(gt => gt.GoalId == goal.Id);
            var allTasksCompleted = true;

            foreach (var goalTask in goalTasks)
                if (!goalTask.Completed)
                    allTasksCompleted = false;

            goal.AllTasksCompleted = allTasksCompleted;

            await Context.SaveChangesAsync();
        }
    }
}