using System;
using Google.Android.Material.BottomNavigation;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace GoalTracker.Droid.PlatformServices.Shell
{
    public class CustomBottomNavigationAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public CustomBottomNavigationAppearanceTracker(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
        {
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            try
            {
                base.SetAppearance(bottomView, appearance);
                bottomView.ItemIconTintList = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}