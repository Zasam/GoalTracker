using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalAppointmentViewModel : BaseViewModel, IGoalAppointmentViewModel
    {
        private readonly IGoalAppointmentRepository goalAppointmentRepository;

        private GoalAppointment[] goalAppointments;
        private GoalAppointment goalAppointment;

        #region Properties

        #region ReadOnly Bindings

        public GoalAppointment[] GoalAppointments
        {
            get => goalAppointments;
            private set
            {
                goalAppointments = value;
                OnPropertyChanged();
            }
        }

        public GoalAppointment SelectedGoalAppointment
        {
            get => goalAppointment;
            private set
            {
                goalAppointment = value;
                OnPropertyChanged();
            }
        }

        public Goal Parent { get; }
        public string ParentTitle => Parent == null ? "N/A" : Parent.Title;

        #endregion // ReadOnly Bindings

        #endregion // Properties

        public GoalAppointmentViewModel(Goal parent, IGoalAppointmentRepository goalAppointmentRepository)
        {
            this.goalAppointmentRepository = goalAppointmentRepository;
            Parent = parent;
        }

        public async Task LoadAppointmentsAsync()
        {
            try
            {
                var appointments = await goalAppointmentRepository.GetAllByParentAsync(Parent);
                GoalAppointments = appointments.OrderBy(ga => ga.AppointmentDate)
                    .ToArray();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task ApproveAppointmentAsync(bool success)
        {
            var dbGoalAppointment = await goalAppointmentRepository.GetByIdAsync(goalAppointment.Id);
            dbGoalAppointment.Approve(success);
            await goalAppointmentRepository.SaveChangesAsync();
            SelectedGoalAppointment = dbGoalAppointment;
        }

        public void SetAppointment(GoalAppointment appointment)
        {
            SelectedGoalAppointment = appointment;
        }
    }
}