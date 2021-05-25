using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.Main.Home.GoalAppointments;
using GoalTracker.Views.Main.Home.GoalTasks;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.Main.Home.Goals
{
    public partial class GoalsPage : ContentPage

    {
        private readonly IGoalViewModel goalViewModel;

        private ISettingViewModel settingViewModel;

        public ISettingViewModel SettingViewModel
        {
            get => settingViewModel;
            private set
            {
                settingViewModel = value;
                OnPropertyChanged();
            }
        }

        private Image deleteSwipeImage;
        private Image editSwipeImage;

        public GoalsPage(IGoalViewModel goalViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                InitializeComponent();

                SettingViewModel = settingViewModel;
                this.goalViewModel = goalViewModel;

                BindingContext = goalViewModel;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #region Events

        #region Page Events

        protected override void OnDisappearing()
        {
            try
            {
                GoalListView.ResetSwipe();
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion Page Events

        #region ToolbarItem Events

        private async void AddGoalToolbarItem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddGoalPage(goalViewModel, settingViewModel));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion // ToolbarItem Events

        #region ListView Events

        private void DeleteGoalRightSwipe_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (deleteSwipeImage == null)
                {
                    deleteSwipeImage = sender as Image;

                    if (deleteSwipeImage?.Parent is View deleteSwipeImageView)
                        deleteSwipeImageView.GestureRecognizers.Add(new TapGestureRecognizer {Command = goalViewModel.DeleteGoalAsyncCommand, CommandParameter = goalViewModel.SelectedGoal});
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void EditGoalRightSwipe_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (editSwipeImage == null)
                {
                    editSwipeImage = sender as Image;

                    if (editSwipeImage?.Parent is View editSwipeImageView)
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += delegate { Navigation.PushAsync(new EditGoalPage(goalViewModel, settingViewModel, goalViewModel.SelectedGoal, goalViewModel.SelectedGoal.GoalTasks.ToArray())); };
                        editSwipeImageView.GestureRecognizers.Add(tapGestureRecognizer);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                var listviewSelectedGoal = (Goal) GoalListView.SelectedItem;
                var swipeSelectedGoal = goalViewModel.Goals[e.ItemIndex];

                if (listviewSelectedGoal == null || listviewSelectedGoal.Id != swipeSelectedGoal.Id)
                {
                    GoalListView.Focus();
                    GoalListView.SelectedItem = swipeSelectedGoal;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ShowGoalAppointmentsTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var selectedGoal = goalViewModel.SelectedGoal;

                if (selectedGoal == null)
                    throw new ArgumentNullException("Selected goal not correctly assigned");

                var goalAppointmentViewModel = new GoalAppointmentViewModel(selectedGoal, goalViewModel.GoalAppointmentRepository);
                var goalAppointmentsPage = new GoalAppointmentsPage(goalAppointmentViewModel, selectedGoal);
                Navigation.PushAsync(goalAppointmentsPage, true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ShowGoalTasksTapGestureRecognizers_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var selectedGoal = goalViewModel.SelectedGoal;

                if (selectedGoal?.GoalTasks != null && selectedGoal.GoalTasks.Any())
                {
                    var goalTaskViewModel = new GoalTaskViewModel(selectedGoal, goalViewModel.GoalTaskRepository);
                    var goalTasksPage = new GoalTasksPage(goalTaskViewModel, selectedGoal);
                    Navigation.PushAsync(goalTasksPage, true);
                }
                else
                {
                    DependencyService.Get<IMessenger>()
                        .ShortMessage("Es sind keine Aufgaben für das Ziel " + selectedGoal.Title + " vorhanden");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion ListView Events

        #endregion // Events
    }
}