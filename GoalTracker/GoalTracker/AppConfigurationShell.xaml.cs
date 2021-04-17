using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.RegistrationShell;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppConfigurationShell : Shell
    {
        public AppConfigurationShell(ISettingViewModel settingViewModel)
        {
            InitializeComponent();

            Tabbar.Items.Add(new ShellContent
            {
                Title = "Konfiguration",
                Icon = "Settings.png",
                Content = new RegistrationPage(settingViewModel),
                Route = "UserSettingsPage"
            });
        }
    }
}