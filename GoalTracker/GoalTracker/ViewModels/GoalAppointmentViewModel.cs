using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GoalTracker.Entities;
using GoalTracker.Services.Interface;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class GoalAppointmentViewModel : BaseViewModel, IGoalAppointmentViewModel
    {
        #region Repositories

        private readonly IGoalAppointmentRepository goalAppointmentRepository;

        public event EventHandler OnApproved;

        #endregion // Repositories

        #region Properties

        private GoalAppointment[] goalAppointments;

        /// <summary>
        /// The collection of appointments associated with the specified parent
        /// </summary>
        public GoalAppointment[] GoalAppointments
        {
            get => goalAppointments;
            set
            {
                goalAppointments = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The last loaded or selected goal appointment
        /// </summary>
        public GoalAppointment SelectedGoalAppointment { get; set; }

        /// <summary>
        /// The parent entity which all appointments are associated with
        /// </summary>
        public Goal Parent { get; }

        private bool isRefreshing;

        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        /// <summary>
        /// Async command to load all appointments into the appointment collection
        /// </summary>
        public ICommand LoadAppointmentsAsyncCommand { get; }

        /// <summary>
        /// Async command to approve a specified appointment
        /// </summary>
        public ICommand ApproveAppointmentAsyncCommand { get; }

        public ICommand DisapproveAppointmentAsyncCommand { get; }

        #endregion // Commands

        #endregion // Properties

        // TODO: Only used to check bindings in xaml
        public GoalAppointmentViewModel()
        {
            throw new InvalidOperationException("Goal appointment view model shouldn't be initialized through parameterless constructor");
        }

        public GoalAppointmentViewModel(Goal parent, IGoalAppointmentRepository goalAppointmentRepository)
        {
            Parent = parent ?? throw new ArgumentNullException("Passed parent goal is null. Please provide a valid goal to instantiate the goal appointment viewmodel.");
            this.goalAppointmentRepository = goalAppointmentRepository;

            LoadAppointmentsAsyncCommand = new Command(async () => await LoadAppointmentsAsync());
            ApproveAppointmentAsyncCommand = new Command<GoalAppointment>(async appointment => await ApproveAppointmentAsync(appointment));
            DisapproveAppointmentAsyncCommand = new Command<GoalAppointment>(async appointment => await DisapproveAppointmentAsync(appointment));
        }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                IsRefreshing = true;
                var appointments = await goalAppointmentRepository.GetAllByParentAsync(Parent);
                GoalAppointments = appointments.OrderBy(ga => ga.AppointmentDate)
                    .ToArray();
                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task DisapproveAppointmentAsync(GoalAppointment appointment)
        {
            try
            {
                var dbGoalAppointment = await goalAppointmentRepository.GetByIdAsync(appointment.Id);
                dbGoalAppointment.Approve(false);
                await goalAppointmentRepository.SaveChangesAsync();
                OnApproved?.Invoke(this, EventArgs.Empty);
                SelectedGoalAppointment = null;
                //await LoadAppointmentsAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task ApproveAppointmentAsync(GoalAppointment appointment)
        {
            try
            {
                var dbGoalAppointment = await goalAppointmentRepository.GetByIdAsync(appointment.Id);
                dbGoalAppointment.Approve(true);
                await goalAppointmentRepository.SaveChangesAsync();
                OnApproved?.Invoke(this, EventArgs.Empty);
                SelectedGoalAppointment = null;
                //await LoadAppointmentsAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}