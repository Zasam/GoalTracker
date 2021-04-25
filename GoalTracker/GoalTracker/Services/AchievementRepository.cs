using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;

namespace GoalTracker.Services
{
    public class AchievementRepository : Repository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(IGoalTrackerContext context)
            : base(context)
        {
        }

        public new async Task<IEnumerable<Achievement>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<Achievement> GetByInternalTag(string internalTag)
        {
            var achievements = await FindAsync(a => a.InternalTag == internalTag);
            return achievements.FirstOrDefault();
        }

        public new async Task AddRangeAsync(IEnumerable<Achievement> achievements)
        {
            User user = null;
            var achievementsToSave = achievements as Achievement[] ?? achievements.ToArray();
            foreach (var achievement in achievementsToSave)
                if (user == null)
                    user = achievement.User;
                else if (user != achievement.User)
                    throw new Exception("Multiple users where defined in the supplied achievements.");

            var savedUser = Context.Users.FirstOrDefault(u => u.Name == user.Name);

            if (savedUser == null)
                throw new Exception("Supplied user(s) couldn't be found in the database. Please create the supplied user and try again");

            await base.AddRangeAsync(achievementsToSave);
        }
    }
}