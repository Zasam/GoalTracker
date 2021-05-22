using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.Services.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserAsync();
        Task AddUserAsync(User user);
        Task ChangeUsernameAsync(string newName);
    }
}