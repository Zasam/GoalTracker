using System.Collections.Generic;
using GoalTracker.Entities;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        private List<Achievement> achievements;

        public List<Achievement> Achievements
        {
            get => achievements;
            set
            {
                achievements = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenLinkCommand => new Command<string>(OpenLink);

        private void OpenLink(string url)
        {
            Launcher.OpenAsync(url);
        }
    }
}