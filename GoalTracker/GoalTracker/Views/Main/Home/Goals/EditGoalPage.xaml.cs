using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.Utilities;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.Pickers;
using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Home.Goals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGoalPage : ContentPage
    {
        private readonly GoalTask[] goalTasks;
        private readonly Goal goalToEdit;
        private readonly string username;
        private readonly IGoalViewModel viewModel;
        private bool contentLoaded;
        private int goalTaskCounter;
        private bool saving;

        public EditGoalPage(IGoalViewModel viewModel, Goal goal, GoalTask[] goalTasks, string username)
        {
            goalToEdit = goal;
            this.goalTasks = goalTasks;
            this.username = username;
            this.viewModel = viewModel;

            contentLoaded = false;

            BindingContext = viewModel;

            InitializeComponent();

            Title = $"Ziel: {goal.Title} bearbeiten";
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                saving = false;
                InitializeComponentValues(goalToEdit, goalTasks);
                contentLoaded = true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void InitializeComponentValues(Goal goal, GoalTask[] goalTasks)
        {
            try
            {
                GoalTitleEntry.Text = goal.Title;
                GoalNotesEditor.Text = goal.Notes;
                GoalStartDatePicker.Date = goal.StartDate;
                GoalEndDatePicker.Date = goal.EndDate;
                GoalHasDueDateCheckBox.IsChecked = goal.HasDueDate;
                GoalNotificationTimePicker.Time = goal.NotificationTime;
                GoalNotificationIntervalPicker.SelectedIndex = (int) goal.GoalAppointmentInterval;
                InitializeGoalTasksComponent(goalTasks);
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
                var valid = ValidateInputs(goalToEdit);
                if (!valid)
                    return;

                saving = true;

                var goal = await viewModel.EditGoalAsync(goalToEdit, GetTasks(goalToEdit), username);

                if (goal != null)
                {
                    //TODO: Achievement unlockable for first editing of goal?
                    var messenger = DependencyService.Get<IMessenger>();
                    messenger.LongMessage($"Ziel: {goalToEdit.Title} wurde erfolgreich bearbeitet.");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private bool ValidateInputs(Goal input)
        {
            var valid = Validator.ValidateGoalInputs(input);
            GoalTitleTextInputLayout.HasError = !valid;
            ErrorTextLabel.IsVisible = !valid;
            return valid;
        }

        private void GoalNotificationTimePicker_TimeSelected(object sender, TimeChangedEventArgs e)
        {
            try
            {
                var newValue = (TimeSpan) e.NewValue;

                var hour = DateTime.Now.Hour;
                var minute = DateTime.Now.Minute;
                if ((newValue < TimeSpan.FromHours(hour) || newValue.Hours == hour && newValue.Minutes <= minute) &&
                    GoalStartDatePicker.Date.Day <= DateTime.Now.Day &&
                    GoalStartDatePicker.Date.Month <= DateTime.Now.Month && !saving && contentLoaded)
                {
                    GoalNotificationTimePicker.Time = new TimeSpan(hour + 1, 00, 00);
                    DependencyService.Get<IMessenger>().ShortMessage("Bitte wähle einen Zeitpunkt in der Zukunft aus.");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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
            if (goalTaskCounter > 0)
            {
                GoalTaskStackLayout.Children.RemoveAt(goalTaskCounter - 1);
                goalTaskCounter--;
            }

            if (goalTaskCounter == 0)
                RemoveGoalTaskButton.IsVisible = false;
        }

        private GoalTask[] GetTasks(Goal parent)
        {
            try
            {
                var goalTasks = new GoalTask[goalTaskCounter];

                if (goalTaskCounter > 0)
                    for (var i = 0; i <= goalTaskCounter - 1; i++)
                    {
                        var taskChildLayout = GoalTaskStackLayout.Children[i] as StackLayout;
                        var taskTitleTextInputLayout = taskChildLayout?.Children[0] as SfTextInputLayout;
                        var taskTitleEntry = taskTitleTextInputLayout?.InputView as Entry;
                        var title = taskTitleEntry == null ? string.Empty : taskTitleEntry.Text;
                        goalTasks[i] = new GoalTask(parent, title, string.Empty, false);
                    }

                return goalTasks;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        private void InitializeGoalTasksComponent(GoalTask[] goalTasks)
        {
            try
            {
                if (goalTasks.Any())
                {
                    foreach (var goalTask in goalTasks)
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
                        taskTitleEntry.Text = goalTask.Title;
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
            // TODO: Implement check if a emoji was selected
            var text = GoalImageEntry.Text;
            if (text.Length != 2)
                GoalImageEntry.Text = string.Empty;
        }
    }
}