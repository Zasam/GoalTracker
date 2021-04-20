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
        private readonly IGoalTrackerContext context;

        public UserRepository(IGoalTrackerContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<User> GetUserAsync()
        {
            try
            {
                var users = await GetAllAsync();

                if (users != null)
                {
                    var userCollection = users as User[] ?? users.ToArray();
                    if (userCollection.Any())
                        return userCollection.FirstOrDefault();
                }
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
            try
            {
                var users = await GetAllAsync();

                var userCollection = users as User[] ?? users.ToArray();
                if (userCollection.Any())
                {
                    var user = userCollection.FirstOrDefault();

                    if (user != null)
                    {
                        user.Name = name;
                        await SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}