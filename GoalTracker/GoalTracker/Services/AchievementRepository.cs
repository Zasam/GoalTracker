using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public class AchievementRepository : Repository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(IGoalTrackerContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Achievement>> GetByTitleAsync(string title)
        {
            return await FindAsync(a => a.Title == title);
        }
    }
}