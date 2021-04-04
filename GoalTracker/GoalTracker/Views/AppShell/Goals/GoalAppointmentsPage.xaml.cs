using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.AppShell.Goals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalAppointmentsPage : ContentPage
    {
        private readonly IGoalAppointmentRepository goalAppointmentRepository;
        private readonly IGoalAppointmentViewModel viewModel;
        private Image approveGoalAppointmentFailureImage;
        private Image approveGoalAppointmentSuccessImage;
        private int itemIndex;

        public GoalAppointmentsPage(IGoalAppointmentViewModel viewModel,
            IGoalAppointmentRepository goalAppointmentRepository)
        {
            InitializeComponent();

            this.goalAppointmentRepository = goalAppointmentRepository;
            this.viewModel = viewModel;
            BindingContext = viewModel;
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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

        private void RefreshGoalAppointments()
        {
            GoalAppointmentListViewPullToRefresh.IsRefreshing = true;
            viewModel.LoadAppointments();
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
                var dbGoalAppointment = await goalAppointmentRepository.GetAsync(viewModel.SelectedGoalAppointment.Id);
                dbGoalAppointment.Approve(success);
                viewModel.SelectedGoalAppointment = dbGoalAppointment;
                RefreshGoalAppointments();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void GoalAppointmentListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                itemIndex = e.ItemIndex;

                var goalAppointmentTask = await goalAppointmentRepository.GetAllByParentAsync(viewModel.Parent);
                var goalAppointments = goalAppointmentTask.OrderBy(ga => ga.AppointmentDate).ToArray();

                viewModel.SelectedGoalAppointment = goalAppointments[itemIndex];

                if (viewModel.SelectedGoalAppointment != null &&
                    viewModel.SelectedGoalAppointment.AppointmentDate >= DateTime.Now)
                {
                    e.Cancel = true;
                    itemIndex = -1;
                    viewModel.SelectedGoalAppointment = null;
                    DependencyService.Get<IMessenger>()
                        .ShortMessage("Dieser Termin kann noch nicht validiert werden, da er in der Zukunft liegt.");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void GoalAppointmentListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            try
            {
                itemIndex = e.ItemIndex;
                var goalAppointmentTask = await goalAppointmentRepository.GetAllByParentAsync(viewModel.Parent);
                var goalAppointments = goalAppointmentTask.OrderBy(ga => ga.AppointmentDate).ToArray();
                viewModel.SelectedGoalAppointment = goalAppointments[itemIndex];
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }
    }
}