using System.Threading.Tasks;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalAppointmentViewModel
    {
        public Goal Parent { get; }
        public string ParentTitle { get; }
        public GoalAppointment[] GoalAppointments { get; }
        public GoalAppointment SelectedGoalAppointment { get; set; }
        public Task LoadAppointmentsAsync();
        public Task ApproveAppointmentAsync(bool success);
    }
}