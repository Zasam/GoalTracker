using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public interface IAchievementRepository : IRepository<Achievement>
    {
        public Task<IEnumerable<Achievement>> GetByTitleAsync(string title);
    }
}