using System;
using System.Collections.Generic;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.AppShell.Goals
{
    public partial class GoalsPage : ContentPage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly IGoalAppointmentRepository goalAppointmentRepository;
        private readonly IGoalRepository goalRepository;
        private readonly IGoalTaskRepository goalTaskRepository;
        private readonly string username;
        private readonly IGoalViewModel viewModel;
        private readonly string welcomeMessage;
        private Image deleteSwipeImage;
        private Image editSwipeImage;
        private int itemIndex;

        public GoalsPage(IGoalRepository goalRepository, IGoalAppointmentRepository goalAppointmentRepository,
            IAchievementRepository achievementRepository, IGoalViewModel viewModel,
            IGoalTaskRepository goalTaskRepository, string username)
        {
            InitializeComponent();

            this.goalRepository = goalRepository;
            this.goalAppointmentRepository = goalAppointmentRepository;
            this.achievementRepository = achievementRepository;
            this.goalTaskRepository = goalTaskRepository;
            this.username = username;
            this.viewModel = viewModel;
            BindingContext = viewModel;
            welcomeMessage = GetRandomWelcomeMessage(username);
        }

        //TODO: Add option to show listview with history of approvals
        protected override async void OnAppearing()
        {
            try
            {
                RefreshGoals();

                var achievementCollection = await achievementRepository.GetAllAsync();
                var achievements = achievementCollection as Achievement[] ?? achievementCollection.ToArray();
                double unlockedAchievementsCount = achievements.Count(a => a.Unlocked);
                double achievementsCount = achievements.Count();
                viewModel.AchievementProgress = Math.Round(unlockedAchievementsCount / achievementsCount * 100, 0);
                viewModel.AchievementProgressPoints = achievements.Where(a => a.Unlocked).Sum(a => a.Experience);
                UsernameLabel.Text = welcomeMessage;

                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private string GetRandomWelcomeMessage(string username)
        {
            var random = new Random().Next(0, 3);

            switch (random)
            {
                case 0:
                    return $"Na {username}, wie geht's? 😄";
                case 1:
                    return $"Hey {username}, schöner Tag oder? 😍";
                case 2:
                    return $"Schön das du wieder an deinen Zielen dran bist {username} 🎯";
                case 3:
                    return $"Ich hoffe dir gefällt {nameof(GoalTracker)}, {username} ♥️";
                default:
                    return $"Irgendwas ist schief gelaufen {username}, wenn was schief geht, starte die App neu 😰 ";
            }
        }

        private async void AddGoalToolbarItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGoalPage(viewModel, goalRepository, goalAppointmentRepository,
                achievementRepository, goalTaskRepository, username));
        }

        private void GoalListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                GoalListViewPullToRefresh.IsRefreshing = true;

                RefreshGoals();
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

        private async void RefreshGoals()
        {
            try
            {
                GoalListViewPullToRefresh.IsRefreshing = true;
                var goalTask = await goalRepository.GetAllAsync();

                var goals = goalTask as List<Goal> ?? goalTask.ToList();
                foreach (var goal in goals)
                {
                    var associatedGoalDates = await goalAppointmentRepository.GetAllByParentAsync(goal);
                    var lastGoalApproval = associatedGoalDates.GetLastGoalApproval();
                    //TODO: Unused variable here???!?
                }

                viewModel.Goals = goals;
                GoalListViewPullToRefresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void DeleteGoal()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoal = viewModel.Goals[itemIndex];
                    await goalRepository.RemoveAsync(selectedGoal);
                    DependencyService.Get<IMessenger>().LongMessage($"Ziel {selectedGoal.Title} erfolgreich gelöscht.");
                    DependencyService.Get<INotificationQueueManager>().CancelAlarms(selectedGoal);

                    GoalListView.ResetSwipe();
                    RefreshGoals();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void EditGoal()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoal = viewModel.Goals[itemIndex];

                    GoalListView.ResetSwipe();
                    await Navigation.PushAsync(new EditGoalPage(viewModel, goalRepository, goalTaskRepository,
                        selectedGoal, username));
                }
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
            GoalListView.ResetSwipe();

            base.OnDisappearing();
        }

        private async void GoalListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                var goalTask = await goalRepository.GetAllAsync();
                var goals = goalTask.ToArray();

                var listviewSelectedGoal = (Goal) GoalListView.SelectedItem;
                var swipeSelectedGoal = goals[e.ItemIndex];

                if (listviewSelectedGoal == null || swipeSelectedGoal == null ||
                    listviewSelectedGoal.Id != swipeSelectedGoal.Id)
                {
                    e.Cancel = true;
                    DependencyService.Get<IMessenger>()
                        .ShortMessage("Bitte wähle ein Ziel aus, um die erweiterten Optionen anzuzeigen.");
                }


                itemIndex = -1;
                viewModel.SelectedGoal = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void GoalListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            itemIndex = e.ItemIndex;
        }

        private void ShowGoalAppointmentsTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var selectedGoal = viewModel.SelectedGoal;
            var goalAppointmentViewModel = new GoalAppointmentViewModel(selectedGoal, goalAppointmentRepository);
            var goalAppointmentsPage = new GoalAppointmentsPage(goalAppointmentViewModel, goalAppointmentRepository);
            Navigation.PushAsync(goalAppointmentsPage, true);
        }

        private void ShowGoalTasksTapGestureRecognizers_OnTapped(object sender, EventArgs e)
        {
            var selectedGoal = viewModel.SelectedGoal;
            var goalTaskViewModel = new GoalTaskViewModel(selectedGoal, goalTaskRepository);
            var goalTasksPage = new GoalTasksPage(goalTaskViewModel, goalTaskRepository);
            Navigation.PushAsync(goalTasksPage, true);
        }

        private void GoalListView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Any())
                    viewModel.SelectedGoal = (Goal) e.AddedItems[0];
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