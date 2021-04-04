using Android.Widget;
using GoalTracker.Droid.PlatformServices.Messaging;
using GoalTracker.PlatformServices;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(Messenger))]

namespace GoalTracker.Droid.PlatformServices.Messaging
{
    public class Messenger : IMessenger
    {
        public void LongMessage(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortMessage(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}