using System;
using GoalTracker.Droid.Renderers;
using GoalTracker.Views.Initialization;
using Microsoft.AppCenter.Crashes;
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
            try
            {
                base.OnAttachedToWindow();
                MessagingCenter.Send(new object(), "ViewLoaded");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}