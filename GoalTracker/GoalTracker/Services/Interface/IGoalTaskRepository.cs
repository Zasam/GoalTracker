using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services.Interface
{
    public interface IGoalTaskRepository : IRepository<GoalTask>
    {
        Task RemoveAsync(GoalTask goalTask);
        Task RemoveRangeAsync(IEnumerable<GoalTask> goalTasks);
        Task AddAsync(GoalTask goalTask);
        Task AddRangeAsync(IEnumerable<GoalTask> goalTasks);
        Task<IEnumerable<GoalTask>> GetAllByParentAsync(Goal parent);
    }
}