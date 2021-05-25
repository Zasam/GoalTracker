using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.Main.Home.GoalAppointments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalAppointmentsPage : ContentPage
    {
        private readonly IGoalAppointmentViewModel viewModel;
        private Image approveGoalAppointmentFailureImage;
        private Image approveGoalAppointmentSuccessImage;

        public GoalAppointmentsPage(IGoalAppointmentViewModel viewModel, Goal parent)
        {
            try
            {
                InitializeComponent();

                this.viewModel = viewModel;
                BindingContext = viewModel;

                this.viewModel.OnApproved += ViewModelOnOnApproved;

                Title = "Benachrichtigungen für: " + parent.Title;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ViewModelOnOnApproved(object sender, EventArgs e)
        {
            try
            {
                GoalAppointmentListView.ResetSwipe();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                GoalAppointmentListView.ResetSwipe();
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ApproveGoalAppointmentSuccess_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (approveGoalAppointmentSuccessImage == null)
                {
                    approveGoalAppointmentSuccessImage = sender as Image;

                    if (approveGoalAppointmentSuccessImage?.Parent is View approveGoalAppointmentSuccessImageView)
                        approveGoalAppointmentSuccessImageView.GestureRecognizers.Add(new TapGestureRecognizer {Command = viewModel.ApproveAppointmentAsyncCommand, CommandParameter = viewModel.SelectedGoalAppointment});
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ApproveGoalAppointmentFailure_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (approveGoalAppointmentFailureImage == null)
                {
                    approveGoalAppointmentFailureImage = sender as Image;

                    if (approveGoalAppointmentFailureImage?.Parent is View approveGoalAppointmentFailureImageView)
                        approveGoalAppointmentFailureImageView.GestureRecognizers.Add(new TapGestureRecognizer {Command = viewModel.DisapproveAppointmentAsyncCommand, CommandParameter = viewModel.SelectedGoalAppointment});
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalAppointmentListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                var goalAppointments = viewModel.GoalAppointments.OrderBy(ga => ga.AppointmentDate).ToArray();

                var listviewSelectedGoalAppointment = (GoalAppointment) GoalAppointmentListView.SelectedItem;
                var swipeSelectedGoalAppointment = goalAppointments[e.ItemIndex];

                if (swipeSelectedGoalAppointment != null && swipeSelectedGoalAppointment.AppointmentDate > DateTime.Now)
                {
                    //TODO: No message shown???
                    DependencyService.Get<IMessenger>().ShortMessage("Der Termin kann noch nicht bestätigt werden, da er noch in der Zukunft liegt.");
                    e.Cancel = true;
                    //GoalAppointmentListView.ResetSwipe();
                }

                if (listviewSelectedGoalAppointment == null || listviewSelectedGoalAppointment.Id != swipeSelectedGoalAppointment?.Id)
                {
                    GoalAppointmentListView.Focus();
                    GoalAppointmentListView.SelectedItem = swipeSelectedGoalAppointment;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}