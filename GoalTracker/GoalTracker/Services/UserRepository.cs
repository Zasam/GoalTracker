using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;

namespace GoalTracker.Services
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IGoalTrackerContext context)
            : base(context)
        {
        }

        public async Task<string> GetUsernameAsync()
        {
            var users = await GetAllAsync();

            var userCollection = users as User[] ?? users.ToArray();
            if (userCollection.Any())
                return userCollection.FirstOrDefault()?.Name;

            return string.Empty;
        }
    }
}