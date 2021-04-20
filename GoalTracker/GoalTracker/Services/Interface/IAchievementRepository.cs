using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services.Interface
{
    public interface IAchievementRepository : IRepository<Achievement>
    {
        Task<IEnumerable<Achievement>> GetAllAsync();
        Task<Achievement> GetByInternalTag(string internalTag);
        Task AddRangeAsync(IEnumerable<Achievement> achievements);
    }
}