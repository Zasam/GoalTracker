using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}