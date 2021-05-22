using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Services
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IGoalTrackerContext context)
            : base(context)
        {
        }

        public async Task<User> GetUserAsync()
        {
            try
            {
                var users = await GetAllAsync();

                if (users != null)
                    return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return null;
        }

        public async Task AddUserAsync(User user)
        {
            var savedUser = await GetUserAsync();

            if (savedUser == null)
                await AddAsync(user);
            else
                throw new Exception("WARNING: Only one user should be registered, you are trying to add a second user to the database.");
        }

        public async Task ChangeUsernameAsync(string name)
        {
            var user = await GetUserAsync();

            if (user == null)
                throw new InvalidOperationException("WARNING: You are trying to change the username of a user, but no registered user could be found.");

            user.Name = name;
            await SaveChangesAsync();
        }
    }
}