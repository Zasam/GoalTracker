using System;
using System.Threading.Tasks;
using BQFramework.Tasks;
using GoalTracker.Entities;
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
        private bool isInitialized;
        private User newUser;

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
                if (!isInitialized)
                {
                    isInitialized = true;

                    await SetInitializationUI("Datenbank wird initialisiert...", 33, Easing.Linear);
                    newUser = AsyncHelper.RunSync(() => settingViewModel.RegisterDefaultUserAsync());

                    await SetInitializationUI("Benutzer wird angelegt...", 66, Easing.Linear);
                    AsyncHelper.RunSync(() => settingViewModel.CreateAchievementsAsync(newUser));

                    await SetInitializationUI("Erfolge werden erstellt...", 100, Easing.Linear);
                }
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
            GoalTracker.AppShell.Instance.SetUIState(newUser, UIStates.Initialization, UIStates.Configuration);
        }
    }
}