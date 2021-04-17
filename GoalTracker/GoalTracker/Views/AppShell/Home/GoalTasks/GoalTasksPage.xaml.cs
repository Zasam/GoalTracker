using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.AppShell.Home.GoalTasks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalTasksPage : ContentPage
    {
        private readonly IGoalTaskViewModel viewModel;
        private Image deleteSwipeImage;
        private Image editSwipeImage;
        private int itemIndex;

        public GoalTasksPage(IGoalTaskViewModel viewModel, Goal parent)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            BindingContext = viewModel;
            Title = "Aufgaben für: " + parent.Title;
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                await viewModel.LoadTasksAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void GoalTaskListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                GoalTaskListViewPullToRefresh.IsRefreshing = true;
                viewModel.LoadTasksAsync();
                DependencyService.Get<IMessenger>().ShortMessage("Deine Aufgaben wurden erfolgreich aktualisiert.");
                GoalTaskListViewPullToRefresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void GoalTaskListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                await viewModel.LoadTasksAsync();

                var listviewSelectedGoalTask = (Goal) GoalTaskListView.SelectedItem;
                var swipeSelectedGoalTask = viewModel.GoalTasks[e.ItemIndex];

                if (listviewSelectedGoalTask == null || listviewSelectedGoalTask.Id != swipeSelectedGoalTask.Id)
                {
                    GoalTaskListView.Focus();
                    GoalTaskListView.SelectedItem = swipeSelectedGoalTask;
                    viewModel.SelectedGoalTask = swipeSelectedGoalTask;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalTaskListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
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

        private async void DeleteGoalTask()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoalTask = viewModel.GoalTasks[itemIndex];
                    await viewModel.DeleteTaskAsync(selectedGoalTask);
                    DependencyService.Get<IMessenger>()
                        .LongMessage($"Aufgabe {selectedGoalTask.Title} erfolgreich gelöscht.");

                    GoalTaskListView.ResetSwipe();
                    await viewModel.LoadTasksAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void EditGoalTask()
        {
            //TODO: Implement editing of goal tasks!!!
        }

        private void EditGoalTaskRightSwipe_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (editSwipeImage == null)
                {
                    editSwipeImage = sender as Image;

                    if (editSwipeImage?.Parent is View editSwipeImageView)
                    {
                        editSwipeImageView.GestureRecognizers.Add(new TapGestureRecognizer
                            {Command = new Command(EditGoalTask)});
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

        private void DeleteGoalTaskRightSwipe_OnBindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (deleteSwipeImage == null)
                {
                    deleteSwipeImage = sender as Image;

                    if (deleteSwipeImage?.Parent is View deleteSwipeImageView)
                    {
                        deleteSwipeImageView.GestureRecognizers.Add(new TapGestureRecognizer
                            {Command = new Command(DeleteGoalTask)});
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

        private async void GoalTaskCompletionCommandTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            try
            {
                await viewModel.SetTaskCompletedAsync();
                GoalTaskListView.ResetSwipe();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void GoalTaskListView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Any())
                    viewModel.SelectedGoalTask = (GoalTask) e.AddedItems[0];
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}