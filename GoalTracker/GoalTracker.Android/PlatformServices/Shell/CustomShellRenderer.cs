using GoalTracker.Droid.PlatformServices.Shell;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(CustomShellRenderer))]

namespace GoalTracker.Droid.PlatformServices.Shell
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new CustomBottomNavigationAppearanceTracker(this, shellItem);
        }
    }
}