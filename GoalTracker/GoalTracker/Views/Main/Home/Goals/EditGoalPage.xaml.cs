using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.Utilities;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Home.Goals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGoalPage : ContentPage
    {
        public ISettingViewModel SettingViewModel { get; }

        private readonly GoalTask[] goalTasks;
        private readonly IGoalViewModel goalViewModel;
        private readonly Goal goalToEdit;
        private int goalTaskCounter;

        public EditGoalPage(IGoalViewModel goalViewModel, ISettingViewModel settingViewModel, Goal goal, GoalTask[] goalTasks)
        {
            try
            {
                SettingViewModel = settingViewModel;
                goalToEdit = goal;
                this.goalTasks = goalTasks;
                this.goalViewModel = goalViewModel;
                BindingContext = goalViewModel;
                Title = $"Ziel: {goal.Title} bearbeiten";

                InitializeComponent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                InitializeComponentValues(goalToEdit, goalTasks);
                AchievementStackLayout.InitializeAchievementAnimation();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void InitializeComponentValues(Goal goal, GoalTask[] tasks)
        {
            try
            {
                goalViewModel.GoalTitle = goal.Title;
                goalViewModel.GoalNotes = goal.Notes;
                goalViewModel.GoalStartDate = goal.StartDate;
                goalViewModel.GoalHasDueDate = goal.HasDueDate;
                goalViewModel.GoalEndDate = goal.EndDate;
                goalViewModel.GoalNotificationTime = goal.NotificationTime;
                goalViewModel.GoalNotificationIntervalIndex = (int) goal.GoalAppointmentInterval;
                goalViewModel.GoalImage = goal.DetailImage;
                InitializeGoalTasksComponent(tasks);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void SaveGoalButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                //TODO: Validate in command EditGoalAsyncCommand in viewmodel!
                var valid = ValidateInputs(goalToEdit);

                var messenger = DependencyService.Get<IMessenger>();
                messenger.LongMessage($"Ziel: {goalToEdit.Title} wurde erfolgreich bearbeitet.");
                await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel, AchievementProgressBar, "Erfolg freigeschaltet: Dein erstes Ziel 🚀 bearbeitet" + Environment.NewLine + "Du hast dein erstes Ziel bearbeitet, toll :)");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private bool ValidateInputs(Goal input)
        {
            try
            {
                var valid = Validator.ValidateGoalInputs(input);
                GoalTitleTextInputLayout.HasError = !valid;
                ErrorTextLabel.IsVisible = !valid;
                return valid;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        private void AddGoalTaskButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                goalTaskCounter++;

                var childLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                var syncfusionTextInputLayout = new SfTextInputLayout
                {
                    Hint = "Titel",
                    ContainerType = ContainerType.Filled,
                    OutlineCornerRadius = 8,
                    ContainerBackgroundColor = Color.FromHex("C4C4C4"),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(0, 0, 0, 5),
                    BackgroundColor = Color.FromHex("EFEFEF"),
                    HelperText = "Aufgabe Nr. " + goalTaskCounter,
                    FocusedColor = Color.FromHex("86B5FC")
                };

                var taskTitleEntry = new Entry();
                syncfusionTextInputLayout.InputView = taskTitleEntry;
                childLayout.Children.Add(syncfusionTextInputLayout);
                GoalTaskStackLayout.Children.Add(childLayout);

                RemoveGoalTaskButton.IsVisible = true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void RemoveGoalTaskButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                if (goalTaskCounter > 0)
                {
                    GoalTaskStackLayout.Children.RemoveAt(goalTaskCounter - 1);
                    goalTaskCounter--;
                }

                if (goalTaskCounter == 0)
                    RemoveGoalTaskButton.IsVisible = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        //TODO: Implement this in viewModel command EditGoalAsyncCommand!
        //private GoalTask[] GetTasks(Goal parent)
        //{
        //    try
        //    {
        //        var tasks = new GoalTask[goalTaskCounter];

        //        if (goalTaskCounter > 0)
        //            for (var i = 0; i <= goalTaskCounter - 1; i++)
        //            {
        //                var taskChildLayout = GoalTaskStackLayout.Children[i] as StackLayout;
        //                var taskTitleTextInputLayout = taskChildLayout?.Children[0] as SfTextInputLayout;
        //                var title = !(taskTitleTextInputLayout?.InputView is Entry taskTitleEntry) ? string.Empty : taskTitleEntry.Text;
        //                tasks[i] = new GoalTask(parent, title, string.Empty, false);
        //            }

        //        return tasks;
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex);
        //        return null;
        //    }
        //}

        private void InitializeGoalTasksComponent(GoalTask[] tasks)
        {
            try
            {
                if (tasks.Any())
                {
                    foreach (var goalTask in tasks)
                    {
                        goalTaskCounter++;

                        var childLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };

                        var syncfusionTextInputLayout = new SfTextInputLayout
                        {
                            Hint = "Titel",
                            ContainerType = ContainerType.Filled,
                            OutlineCornerRadius = 8,
                            ContainerBackgroundColor = Color.FromHex("C4C4C4"),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Margin = new Thickness(0, 0, 0, 5),
                            BackgroundColor = Color.FromHex("EFEFEF"),
                            HelperText = "Aufgabe Nr. " + goalTaskCounter,
                            FocusedColor = Color.FromHex("86B5FC")
                        };

                        var taskTitleEntry = new Entry {Text = goalTask.Title};
                        syncfusionTextInputLayout.InputView = taskTitleEntry;
                        childLayout.Children.Add(syncfusionTextInputLayout);
                        GoalTaskStackLayout.Children.Add(childLayout);
                    }

                    RemoveGoalTaskButton.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void GoalImageEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // TODO: Implement check if a emoji was selected
                var text = GoalImageEntry.Text;
                if (text.Length != 2)
                    GoalImageEntry.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void AchievementProgressBar_OnProgressCompleted(object sender, ProgressValueEventArgs e)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}