using System;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeStartedEventArgs = Syncfusion.ListView.XForms.SwipeStartedEventArgs;

namespace GoalTracker.Views.Main.Home.GoalTasks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalTasksPage : ContentPage
    {
        private readonly IGoalTaskViewModel goalTaskViewModel;
        private Image deleteSwipeImage;
        private Image editSwipeImage;
        private Image setTaskCompletedImage;

        public GoalTasksPage(IGoalTaskViewModel goalTaskViewModel, Goal parent)
        {
            InitializeComponent();

            this.goalTaskViewModel = goalTaskViewModel;
            BindingContext = goalTaskViewModel;
            Title = "Aufgaben für: " + parent.Title;
        }

        private void GoalTaskListViewPullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                GoalTaskListViewPullToRefresh.IsRefreshing = true;
                DependencyService.Get<IMessenger>().ShortMessage("Deine Aufgaben wurden erfolgreich aktualisiert.");
                GoalTaskListViewPullToRefresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalTaskListView_OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            try
            {
                var listviewSelectedGoalTask = (Goal) GoalTaskListView.SelectedItem;
                var swipeSelectedGoalTask = goalTaskViewModel.GoalTasks[e.ItemIndex];

                if (listviewSelectedGoalTask == null || listviewSelectedGoalTask.Id != swipeSelectedGoalTask.Id)
                {
                    GoalTaskListView.Focus();
                    GoalTaskListView.SelectedItem = swipeSelectedGoalTask;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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
                            {Command = goalTaskViewModel.DeleteTaskAsyncCommand, CommandParameter = goalTaskViewModel.SelectedGoalTask});
                        deleteSwipeImage.Source = ImageSource.FromFile("Delete.png");
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void SetTaskCompletedLeftSwipeImage_OnBindingContextChanged(object sender, EventArgs e)
        {
            if (setTaskCompletedImage == null)
            {
                setTaskCompletedImage = sender as Image;

                if (setTaskCompletedImage?.Parent is View setTaskCompletedImageView)
                    setTaskCompletedImageView.GestureRecognizers.Add(new TapGestureRecognizer {Command = goalTaskViewModel.SetTaskCompletedAsyncCommand, CommandParameter = goalTaskViewModel.SelectedGoalTask});
            }
        }
    }
}