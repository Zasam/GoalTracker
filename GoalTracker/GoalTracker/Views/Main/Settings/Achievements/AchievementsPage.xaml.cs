using System;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Settings.Achievements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsPage : ContentPage
    {
        public AchievementsPage(ISettingViewModel settingViewModel)
        {
            try
            {
                BindingContext = settingViewModel;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}