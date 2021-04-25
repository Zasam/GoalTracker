using GoalTracker.Droid.Renderers;
using GoalTracker.Views.Initialization;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(InitializationPage), typeof(InitializationPageRenderer))]

namespace GoalTracker.Droid.Renderers
{
    public class InitializationPageRenderer : PageRenderer
    {
        public InitializationPageRenderer(Android.Content.Context context)
            : base(context)
        {
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            MessagingCenter.Send(new object(), "ViewLoaded");
        }
    }
}