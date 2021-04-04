using GoalTracker.Services;
using GoalTracker.Views.InitializationShell;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializationShell : Shell
    {
        public InitializationShell(IUserRepository userRepository, IAchievementRepository achievementRepository)
        {
            InitializeComponent();

            Tabbar.Items.Add(new InitializationPage(userRepository, achievementRepository));
        }
    }
}