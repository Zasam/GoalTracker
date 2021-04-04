using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.Services;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.ViewModels
{
    public class GoalAppointmentViewModel : BaseViewModel, IGoalAppointmentViewModel
    {
        private readonly IGoalAppointmentRepository goalAppointmentRepository;

        private GoalAppointment goalAppointment;

        private GoalAppointment[] goalAppointments;

        public GoalAppointmentViewModel(Goal parent, IGoalAppointmentRepository goalAppointmentRepository)
        {
            this.goalAppointmentRepository = goalAppointmentRepository;
            Parent = parent;
        }

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

        public string ParentTitle => Parent == null ? "N/A" : Parent.Title;

        public Goal Parent { get; set; }

        public async Task LoadAppointments()
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
    }
}