using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalAppointmentViewModel
    {
        public Goal Parent { get; set; }
        public string ParentTitle { get; }
        public GoalAppointment[] GoalAppointments { get; set; }
        public GoalAppointment SelectedGoalAppointment { get; set; }
        public Task LoadAppointmentsAsync();
        public Task ApproveAppointmentAsync(bool success);
    }
}