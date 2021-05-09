using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;

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
            return await FindAsync(gt => gt.GoalId == parent.Id);
        }

        public new async Task RemoveRangeAsync(IEnumerable<GoalTask> goalTasks)
        {
            await base.RemoveRangeAsync(goalTasks);
        }

        public new async Task AddAsync(GoalTask goalTask)
        {
            await base.AddAsync(goalTask);
        }

        public new async Task AddRangeAsync(IEnumerable<GoalTask> goalTasks)
        {
            var goalTaskCollection = goalTasks as GoalTask[] ?? goalTasks.ToArray();
            foreach (var goalTask in goalTaskCollection)
            {
                var parent = await GetParentAsync(goalTask);
                parent.GoalTaskCount++;
            }

            await base.AddRangeAsync(goalTaskCollection);
        }

        public new async Task RemoveAsync(GoalTask goalTask)
        {
            var parent = await GetParentAsync(goalTask);
            parent.GoalTaskCount = parent.GoalTaskCount > 0 ? parent.GoalTaskCount - 1 : 0;
            await base.RemoveAsync(goalTask);
        }

        private async Task<Goal> GetParentAsync(GoalTask child)
        {
            return await context.Goals.FindAsync(child.GoalId);
        }
    }
}