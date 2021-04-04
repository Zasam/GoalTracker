using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Xamarin.Forms;

namespace GoalTracker.Extensions
{
    public static class UIExtensions
    {
        public static void InitializeAchievementAnimation(this StackLayout achievementStackLayout)
        {
            achievementStackLayout.TranslationY += 100;
            achievementStackLayout.Opacity = 0;
        }

        public static async Task StartAchievementUnlockedAnimation(this StackLayout achievementStackLayout,
            Label achievementLabel, SfCircularProgressBar achievementProgressBar, string achievementUnlockedText)
        {
            try
            {
                achievementLabel.Text = achievementUnlockedText;
                achievementStackLayout.Opacity = 1;
                achievementProgressBar.SetProgress(100, 1000, Easing.Linear);
                await achievementStackLayout.TranslateTo(achievementStackLayout.TranslationX,
                    achievementStackLayout.TranslationY - 100);
                await achievementStackLayout.FadeTo(1, 250U, Easing.Linear);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}