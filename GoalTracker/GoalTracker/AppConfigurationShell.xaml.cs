using GoalTracker.Views.RegistrationShell;
using GoalTracker.ViewModels;
using GoalTracker.Services;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace GoalTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppConfigurationShell : Shell
    {
        public AppConfigurationShell(IUserRepository userRepository, IAchievementRepository achievementRepository, IRegistrationViewModel registrationViewModel)
        {
            InitializeComponent();

            Tabbar.Items.Add(new ShellContent
            {
                Title = "Konfiguration",
                Icon = "Settings.png",
                Content = new RegistrationPage(userRepository, achievementRepository, registrationViewModel),
                Route = "UserSettingsPage"
            });
        }
    }
}