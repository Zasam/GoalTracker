using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels;
using GoalTracker.ViewModels.Interface;
using GoalTracker.Views.AppShell.Home.GoalAppointments;
using GoalTracker.Views.AppShell.Home.GoalTasks;
using Microsoft.AppCenter.Crashes;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.AppShell.Home.Goals
{
    public partial class GoalsPage : ContentPage
    {
        private readonly IGoalViewModel goalViewModel;
        public ISettingViewModel SettingViewModel { get; set; }

        private readonly string username;
        private readonly string welcomeMessage;
        private Image deleteSwipeImage;
        private Image editSwipeImage;
        private int itemIndex;

        public GoalsPage(IGoalViewModel goalViewModel, ISettingViewModel settingViewModel, string username)
        {
            InitializeComponent();

            SettingViewModel = settingViewModel;
            this.goalViewModel = goalViewModel;
            this.username = username;

            BindingContext = goalViewModel;
            welcomeMessage = GetRandomWelcomeMessage(username);
        }

        #region Events

        #region Page Events

        //TODO: Add option to show listview with history of approvals
        protected override async void OnAppearing()
        {
            try
            {
                await RefreshGoals();
                UsernameLabel.Text = welcomeMessage;

                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnDisappearing()
        {
            GoalListView.ResetSwipe();

            base.OnDisappearing();
        }

        #endregion Page Events

        #region PullToRefresh Events

        private async void GoalListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                GoalListViewPullToRefresh.IsRefreshing = true;
                await RefreshGoals();
                DependencyService.Get<IMessenger>().ShortMessage("Deine Ziele wurden erfolgreich aktualisiert.");
                GoalListViewPullToRefresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        #endregion // PullToRefresh Events

        #region ToolbarItem Events

        private async void AddGoalToolbarItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGoalPage(goalViewModel, SettingViewModel, username));
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
                    {
                        deleteSwipeImageView.GestureRecognizers.Add(new TapGestureRecognizer
                            {Command = new Command(DeleteGoal)});
                        deleteSwipeImage.Source = ImageSource.FromFile("Delete.png");
                    }
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
                        editSwipeImageView.GestureRecognizers.Add(new TapGestureRecognizer
                            {Command = new Command(EditGoal)});
                        editSwipeImage.Source = ImageSource.FromFile("Edit.png");
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
                    goalViewModel.SelectedGoal = swipeSelectedGoal;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            itemIndex = e.ItemIndex;
        }

        private void ShowGoalAppointmentsTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var selectedGoal = goalViewModel.SelectedGoal;

                if (selectedGoal == null)
                    throw new ArgumentNullException("Selected goal not correctly assigned");

                var goalAppointmentViewModel =
                    new GoalAppointmentViewModel(selectedGoal, goalViewModel.GoalAppointmentRepository);
                var goalAppointmentsPage =
                    new GoalAppointmentsPage(goalAppointmentViewModel, selectedGoal);
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

        private void GoalListView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Any())
                    goalViewModel.SelectedGoal = (Goal) e.AddedItems[0];
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion ListView Events

        #endregion // Events

        #region Methods

        private string GetRandomWelcomeMessage(string username)
        {
            var random = new Random().Next(0, 3);

            return random switch
            {
                0 => $"Na {username}, wie geht's? 😄",
                1 => $"Hey {username}, schöner Tag oder? 😍",
                2 => $"Schön das du wieder an deinen Zielen dran bist {username} 🎯",
                3 => $"Ich hoffe dir gefällt {nameof(GoalTracker)}, {username} ♥️",
                _ => $"Irgendwas ist schief gelaufen {username}, wenn was schief geht, starte die App neu 😰 "
            };
        }

        private async Task RefreshGoals()
        {
            try
            {
                GoalListViewPullToRefresh.IsRefreshing = true;
                await goalViewModel.LoadGoalsAsync();
                await SettingViewModel.LoadAchievementsAsync();
                var ppoints = SettingViewModel.AchievementProgressPoints;
                var progress = SettingViewModel.AchievementProgress;
                GoalListViewPullToRefresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void DeleteGoal()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoal = goalViewModel.Goals[itemIndex];
                    var delete = await goalViewModel.DeleteGoalAsync(selectedGoal);

                    if (delete)
                    {
                        DependencyService.Get<IMessenger>()
                            .LongMessage($"Ziel {selectedGoal.Title} erfolgreich gelöscht.");
                        DependencyService.Get<INotificationQueueManager>().CancelAlarms(selectedGoal);
                    }

                    GoalListView.ResetSwipe();
                    await RefreshGoals();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void EditGoal()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoal = goalViewModel.Goals[itemIndex];
                    var goalTasks = await goalViewModel.LoadTasksAsync(selectedGoal);
                    GoalListView.ResetSwipe();
                    await Navigation.PushAsync(new EditGoalPage(goalViewModel, selectedGoal, goalTasks, username));
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion // Methods
    }
}