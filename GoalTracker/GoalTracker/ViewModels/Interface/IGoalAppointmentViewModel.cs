using System;
using System.Windows.Input;
using GoalTracker.Entities;

namespace GoalTracker.ViewModels.Interface
{
    public interface IGoalAppointmentViewModel
    {
        GoalAppointment[] GoalAppointments { get; set; }
        GoalAppointment SelectedGoalAppointment { get; set; }
        Goal Parent { get; }
        bool IsRefreshing { get; set; }

        public ICommand LoadAppointmentsAsyncCommand { get; }
        public ICommand ApproveAppointmentAsyncCommand { get; }
        public ICommand DisapproveAppointmentAsyncCommand { get; }
        event EventHandler OnApproved;
    }
}