using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services.Interface
{
    public interface IGoalRepository : IRepository<Goal>
    {
        Task AddAsync(Goal goal);
        Task<IEnumerable<Goal>> GetAllAsync();
        Task<IEnumerable<Goal>> GetAllStartedAsync(DateTime startDate);
        Task<Goal> GetByTitleAsync(string title);
        Task RemoveAsync(Goal goal);
        Task RemoveRangeAsync(IEnumerable<Goal> goals);
        Task UpdateAllTasksCompletedAsync(Goal goal);
    }
}