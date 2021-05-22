using GoalTracker.ViewModels.Interface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Settings.Achievements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsPage : ContentPage
    {
        public AchievementsPage(ISettingViewModel settingViewModel)
        {
            BindingContext = settingViewModel;
            InitializeComponent();
        }
    }
}