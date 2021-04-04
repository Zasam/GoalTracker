using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels
{
    public interface IGoalAppointmentViewModel
    {
        public Goal Parent { get; set; }
        public string ParentTitle { get; }
        public GoalAppointment[] GoalAppointments { get; set; }
        public GoalAppointment SelectedGoalAppointment { get; set; }
        public Task LoadAppointments();
    }
}