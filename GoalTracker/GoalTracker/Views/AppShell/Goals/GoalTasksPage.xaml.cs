using System;
using System.Linq;
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
    public partial class GoalTasksPage : ContentPage
    {
        private readonly IGoalTaskRepository goalTaskRepository;
        private readonly IGoalTaskViewModel viewModel;
        private Image deleteSwipeImage;
        private Image editSwipeImage;
        private int itemIndex;

        public GoalTasksPage(IGoalTaskViewModel viewModel, IGoalTaskRepository goalTaskRepository)
        {
            InitializeComponent();

            this.goalTaskRepository = goalTaskRepository;
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                RefreshGoalTasks();
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void RefreshGoalTasks()
        {
            GoalTaskListViewPullToRefresh.IsRefreshing = true;
            viewModel.LoadTasks();
            GoalTaskListViewPullToRefresh.IsRefreshing = false;
        }

        private void GoalTaskListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            RefreshGoalTasks();
        }

        private void GoalTaskListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            itemIndex = -1;
            viewModel.SelectedGoalTask = null;
        }

        private async void GoalTaskListView_OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            itemIndex = e.ItemIndex;
            var goalTasks = await goalTaskRepository.GetAllAsync();
            viewModel.SelectedGoalTask = goalTasks.ToArray()[itemIndex];
        }

        private async void DeleteGoalTask()
        {
            try
            {
                if (itemIndex >= 0)
                {
                    var selectedGoalTask = viewModel.GoalTasks[itemIndex];
                    await goalTaskRepository.RemoveAsync(selectedGoalTask);
                    DependencyService.Get<IMessenger>()
                        .LongMessage($"Aufgabe {selectedGoalTask.Title} erfolgreich gelöscht.");

                    GoalTaskListView.ResetSwipe();
                    RefreshGoalTasks();
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
                var dbSelectedGoalTask = await goalTaskRepository
                    .GetAllByParentAsync(viewModel.Parent);
                var selectedGoalTask = dbSelectedGoalTask.FirstOrDefault(gt => gt.Id == viewModel.SelectedGoalTask.Id);


                if (selectedGoalTask != null)
                    selectedGoalTask.Completed = !selectedGoalTask.Completed;
                await goalTaskRepository.SaveChangesAsync();
                GoalTaskListView.ResetSwipe();
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