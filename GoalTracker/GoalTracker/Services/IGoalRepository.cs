using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public interface IGoalRepository : IRepository<Goal>
    {
        public new Task<IEnumerable<Goal>> GetAllAsync();
        public Task<IEnumerable<Goal>> GetAllInDateAsync(DateTime date);
        public Task<Goal> GetByTitleAsnyc(string title);
        public new Task RemoveAsync(Goal goal);
        public new Task RemoveRangeAsync(IEnumerable<Goal> goals);
        public Task UpdateAllTasksCompletedAsync(Goal goal);
    }
}