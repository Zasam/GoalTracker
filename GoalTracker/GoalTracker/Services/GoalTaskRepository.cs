using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Services
{
    public class GoalTaskRepository : Repository<GoalTask>, IGoalTaskRepository
    {
        private readonly IGoalTrackerContext context;

        public GoalTaskRepository(IGoalTrackerContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<GoalTask>> GetAllByParentAsync(Goal parent)
        {
            try
            {
                return await FindAsync(gt => gt.GoalId == parent.Id);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public new async Task RemoveRangeAsync(IEnumerable<GoalTask> goalTasks)
        {
            try
            {
                await base.RemoveRangeAsync(goalTasks);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public new async Task AddAsync(GoalTask goalTask)
        {
            try
            {
                await base.AddAsync(goalTask);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public new async Task AddRangeAsync(IEnumerable<GoalTask> goalTasks)
        {
            try
            {
                var goalTaskCollection = goalTasks as GoalTask[] ?? goalTasks.ToArray();
                foreach (var goalTask in goalTaskCollection)
                {
                    var parent = await GetParentAsync(goalTask);
                    parent.GoalTaskCount++;
                }

                await base.AddRangeAsync(goalTaskCollection);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public new async Task RemoveAsync(GoalTask goalTask)
        {
            try
            {
                var parent = await GetParentAsync(goalTask);
                parent.GoalTaskCount = parent.GoalTaskCount > 0 ? parent.GoalTaskCount - 1 : 0;
                await base.RemoveAsync(goalTask);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task<Goal> GetParentAsync(GoalTask child)
        {
            try
            {
                return await context.Goals.FindAsync(child.GoalId);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }
    }
}