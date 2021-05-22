using System;
using System.Threading.Tasks;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Initialization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializationPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;

        public InitializationPage(ISettingViewModel settingViewModel)
        {
            this.settingViewModel = settingViewModel;

            MessagingCenter.Subscribe<object>(new object(), "ViewLoaded", async sender => { await Initialize(); });

            InitializeComponent();
        }

        private async Task Initialize()
        {
            try
            {
                await SetInitializationUI("Datenbank wird initialisiert...", 33, Easing.Linear);
                //TODO: How to use commands in here? Or use initialization command in code behind of view?
                settingViewModel.RegisterDefaultUserAsyncCommand.Execute(null);

                await SetInitializationUI("Benutzer wird angelegt...", 66, Easing.Linear);
                //TODO: How to use commands in here? Or use initialization command in code behind of view?
                settingViewModel.CreateAchievementsAsyncCommand.Execute(null);

                await SetInitializationUI("Erfolge werden erstellt...", 100, Easing.Linear);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task SetInitializationUI(string message, int progress, Easing easing)
        {
            //TODO: Progress incrementation not working correctly when text is changed
            //InitializationMessage.Text = message;
            InitializationProgressBar.SetProgress(progress, 1500, easing);
            await Task.Delay(1500);
        }

        private void InitializationProgressBar_OnProgressCompleted(object sender, ProgressValueEventArgs e)
        {
            AppShell.Instance.SetUIState(UIStates.Initialization, UIStates.Configuration);
        }
    }
}