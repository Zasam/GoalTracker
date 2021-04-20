using System;
using System.Threading.Tasks;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.InitializationShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitializationPage : ContentPage
    {
        private readonly ISettingViewModel settingViewModel;
        private bool isLoaded;

        public InitializationPage(ISettingViewModel settingViewModel)
        {
            this.settingViewModel = settingViewModel;

            InitializeComponent();
        }

        protected override async void LayoutChildren(double x, double y, double width, double height)
        {
            try
            {
                base.LayoutChildren(x, y, width, height);

                await Task.Delay(5000);

                if (!isLoaded)
                {
                    isLoaded = true;

                    await foreach (var progress in settingViewModel.RegisterDefaultUserAsync())
                    {
                        InitializationMessage.Text = progress.Item1;
                        InitializationProgressBar.SetProgress(progress.Item2, 1000, Easing.Linear);
                        await Task.Delay(1000);
                    }

                    App.Instance.ChangeToConfigurationShell();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}