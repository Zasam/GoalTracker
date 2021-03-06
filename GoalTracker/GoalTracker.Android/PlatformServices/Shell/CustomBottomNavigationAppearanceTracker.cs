using Google.Android.Material.BottomNavigation;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

namespace GoalTracker.Droid.PlatformServices.Shell
{
    public class CustomBottomNavigationAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public CustomBottomNavigationAppearanceTracker(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
        {
            
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            base.SetAppearance(bottomView, appearance);
            bottomView.ItemIconTintList = null;
        }
    }
}