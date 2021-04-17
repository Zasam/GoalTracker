using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.InitializationShell;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializationShell : Shell
    {
        public InitializationShell(ISettingViewModel settingViewModel)
        {
            InitializeComponent();

            Tabbar.Items.Add(new InitializationPage(settingViewModel));
        }
    }
}