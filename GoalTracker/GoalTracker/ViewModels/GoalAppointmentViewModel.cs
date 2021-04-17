using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalAppointmentViewModel : BaseViewModel, IGoalAppointmentViewModel
    {
        #region Properties

        #region Repositories

        private readonly IGoalAppointmentRepository goalAppointmentRepository;

        #endregion // Repositories

        private GoalAppointment[] goalAppointments;
        private GoalAppointment goalAppointment;

        public GoalAppointment[] GoalAppointments
        {
            get => goalAppointments;
            set
            {
                goalAppointments = value;
                OnPropertyChanged();
            }
        }

        public GoalAppointment SelectedGoalAppointment
        {
            get => goalAppointment;
            set
            {
                goalAppointment = value;
                OnPropertyChanged();
            }
        }

        public Goal Parent { get; set; }
        public string ParentTitle => Parent == null ? "N/A" : Parent.Title;

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
            var dbGoalAppointment = await goalAppointmentRepository.GetAsync(goalAppointment.Id);
            dbGoalAppointment.Approve(success);
            await goalAppointmentRepository.SaveChangesAsync();
            SelectedGoalAppointment = dbGoalAppointment;
        }
    }
}