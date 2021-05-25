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
            try
            {
                await base.AddAsync(goal);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public new async Task<IEnumerable<Goal>> GetAllAsync()
        {
            try
            {
                return await context.Goals.Include(g => g.GoalAppointments).Include(g => g.GoalTasks).ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Goal>> GetAllStartedAsync(DateTime startDate)
        {
            try
            {
                return await FindAsync(g => g.StartDate <= startDate);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public new async Task RemoveAsync(Goal goal)
        {
            try
            {
                var goalAppointments = context.GoalAppointments.Where(ga => ga.GoalId == goal.Id);
                var goalTasks = context.GoalTasks.Where(gt => gt.GoalId == goal.Id);
                context.GoalAppointments.RemoveRange(goalAppointments);
                context.GoalTasks.RemoveRange(goalTasks);
                await base.RemoveAsync(goal);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public new async Task RemoveRangeAsync(IEnumerable<Goal> goals)
        {
            try
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task<Goal> GetByTitleAsync(string title)
        {
            try
            {
                var goals = await FindAsync(g => g.Title == title);
                return goals.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task UpdateAllTasksCompletedAsync(Goal goal)
        {
            try
            {
                var goalTasks = context.GoalTasks.Where(gt => gt.GoalId == goal.Id);
                var allTasksCompleted = true;

                foreach (var goalTask in goalTasks)
                    if (!goalTask.Completed)
                        allTasksCompleted = false;

                goal.AllTasksCompleted = allTasksCompleted;

                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}