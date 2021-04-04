using System.Collections.Generic;
using GoalTracker.Entities;
using System.Windows.Input;

namespace GoalTracker.ViewModels
{
    public interface ISettingsViewModel
    {
        public List<Achievement> Achievements { get; set; }
        public ICommand OpenLinkCommand { get; }
    }
}