using GoalTracker.Views.AppShell.Settings.Achievements;
using GoalTracker.ViewModels;
using GoalTracker.Services;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace GoalTracker.Views.AppShell.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly IUserRepository userRepository;
        private readonly IAchievementRepository achievementRepository;
        private readonly ISettingsViewModel viewModel;

        public SettingsPage(IUserRepository userRepository, IAchievementRepository achievementRepository, ISettingsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.userRepository = userRepository;
            this.achievementRepository = achievementRepository;

            BindingContext = this.viewModel;

            InitializeComponent();
        }

        private void ShowAchievementsButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AchievementsPage(achievementRepository, viewModel), true);
        }
    }
}