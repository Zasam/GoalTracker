using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Welcome
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                await Task.Delay(5000);
                AppShell.Instance.SetUIState(UIStates.Welcome, UIStates.Initialization);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}