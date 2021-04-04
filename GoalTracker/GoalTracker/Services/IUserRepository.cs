using GoalTracker.Entities;
using System.Threading.Tasks;

namespace GoalTracker.Services
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<string> GetUsernameAsync();
    }
}