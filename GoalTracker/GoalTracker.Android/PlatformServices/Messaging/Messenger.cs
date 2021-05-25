using System;
using Android.Widget;
using GoalTracker.Droid.PlatformServices.Messaging;
using GoalTracker.PlatformServices;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(Messenger))]

namespace GoalTracker.Droid.PlatformServices.Messaging
{
    public class Messenger : IMessenger
    {
        public void LongMessage(string message)
        {
            try
            {
                Toast.MakeText(Application.Context, message, ToastLength.Long)?.Show();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void ShortMessage(string message)
        {
            try
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}