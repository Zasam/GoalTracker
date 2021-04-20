using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.AppShell.Home.GoalAppointments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalAppointmentsPage : ContentPage
    {
        private readonly IGoalAppointmentViewModel viewModel;
        private Image approveGoalAppointmentFailureImage;
        private Image approveGoalAppointmentSuccessImage;
        private int itemIndex;

        public GoalAppointmentsPage(IGoalAppointmentViewModel viewModel, Goal parent)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            BindingContext = viewModel;

            Title = "Benachrichtigungen für: " + parent.Title;
        }

        protected override void OnAppearing()
        {
            try
            {
                RefreshGoalAppointments();
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnDisappearing()
        {
            GoalAppointmentListView.ResetSwipe();

            base.OnDisappearing();
        }

        private void GoalAppointmentListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            RefreshGoalAppointments();
        }

        private async void RefreshGoalAppointments()
        {
            GoalAppointmentListViewPullToRefresh.IsRefreshing = true;
            await viewModel.LoadAppointmentsAsync();
            GoalAppointmentListViewPullToRefresh.IsRefreshing = false;
        }

        private async void ApproveGoalAppointmentSuccess()
        {
            await ApproveGoalAppointment(true);
        }

        private async void ApproveGoalAppointmentFailure()
        {
            await ApproveGoalAppointment(false);
        }

        private async Task ApproveGoalAppointment(bool success)
        {
            try
            {
                await viewModel.ApproveAppointmentAsync(success);
                RefreshGoalAppointments();
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
                    {
                        approveGoalAppointmentSuccessImageView.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(ApproveGoalAppointmentSuccess)
                        });
                        approveGoalAppointmentSuccessImage.Source = ImageSource.FromFile("Success.png");
                    }
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
                    {
                        approveGoalAppointmentFailureImageView.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(ApproveGoalAppointmentFailure)
                        });
                        approveGoalAppointmentFailureImage.Source = ImageSource.FromFile("Failed.png");
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void GoalAppointmentListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                await viewModel.LoadAppointmentsAsync();
                var goalAppointments = viewModel.GoalAppointments.OrderBy(ga => ga.AppointmentDate).ToArray();

                var listviewSelectedGoalAppointment = (GoalAppointment) GoalAppointmentListView.SelectedItem;
                var swipeSelectedGoalAppointment = goalAppointments[e.ItemIndex];

                if (swipeSelectedGoalAppointment != null && swipeSelectedGoalAppointment.AppointmentDate > DateTime.Now)
                {
                    DependencyService.Get<IMessenger>()
                        .ShortMessage("Der Termin kann noch nicht bestätigt werden, da er noch in der Zukunft liegt.");
                    e.Cancel = true;
                    return;
                }

                if (listviewSelectedGoalAppointment == null ||
                    listviewSelectedGoalAppointment.Id != swipeSelectedGoalAppointment.Id &&
                    swipeSelectedGoalAppointment.AppointmentDate <= DateTime.Now)
                {
                    GoalAppointmentListView.Focus();
                    GoalAppointmentListView.SelectedItem = swipeSelectedGoalAppointment;
                    viewModel.SetAppointment(swipeSelectedGoalAppointment);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalAppointmentListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            try
            {
                itemIndex = e.ItemIndex;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalAppointmentListView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Any())
                    viewModel.SetAppointment((GoalAppointment) e.AddedItems[0]);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}