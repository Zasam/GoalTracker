using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public interface IGoalTaskRepository : IRepository<GoalTask>
    {
        public Task<IEnumerable<GoalTask>> GetAllByParentAsync(Goal parent);
        public new Task RemoveAsync(GoalTask goalTask);
        public new Task AddAsync(GoalTask goalTask);
        public new Task AddRangeAsync(IEnumerable<GoalTask> goalTasks);
    }
}