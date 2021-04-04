using GoalTracker.Services;
using GoalTracker.Models;

namespace GoalTracker.ViewModels
{
    public class RegistrationViewModel : BaseViewModel, IRegistrationViewModel
    {
        public string Username { get; set; }

        private readonly IUserRepository userRepository;

        public RegistrationViewModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
    }
}