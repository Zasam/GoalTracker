using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.PlatformServices;
using GoalTracker.Services;
using GoalTracker.Utilities;
using GoalTracker.ViewModels;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.Pickers;
using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.AppShell.Goals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditGoalPage : ContentPage
    {
        private readonly IGoalRepository goalRepository;
        private readonly IGoalTaskRepository goalTaskRepository;
        private readonly Goal goalToEdit;
        private readonly string username;
        private readonly IGoalViewModel viewModel;
        private int goalTaskCounter;
        private bool saving;

        public EditGoalPage(IGoalViewModel viewModel, IGoalRepository goalRepository,
            IGoalTaskRepository goalTaskRepository, Goal goal, string username)
        {
            goalToEdit = goal;
            this.username = username;
            this.viewModel = viewModel;
            this.goalRepository = goalRepository;
            this.goalTaskRepository = goalTaskRepository;

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
                InitializeComponentValues(goalToEdit);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private void InitializeComponentValues(Goal goal)
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
                InitializeGoalTasksComponent(goal);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }

        private async void SaveGoalButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                saving = true;
                goalToEdit.Title = viewModel.GoalTitle;
                goalToEdit.Notes = viewModel.GoalNotes;
                goalToEdit.StartDate = viewModel.GoalStartDate;
                goalToEdit.HasDueDate = viewModel.GoalHasDueDate;
                goalToEdit.EndDate = viewModel.GoalEndDate;
                goalToEdit.GoalAppointmentInterval = viewModel.GoalNotificationInterval;
                goalToEdit.NotificationTime = viewModel.GoalNotificationTime;

                var valid = ValidateInputs(goalToEdit);
                if (!valid)
                    return;

                await goalRepository.SaveChangesAsync();

                var goalTasks = GetTasks(goalToEdit);
                var currentGoalTasks = await goalTaskRepository.GetAllByParentAsync(goalToEdit);
                await goalTaskRepository.RemoveRangeAsync(currentGoalTasks);

                if (goalTasks.Any())
                {
                    foreach (var goalTask in goalTasks)
                        await goalTaskRepository.AddAsync(goalTask);

                    goalToEdit.GoalTaskCount = goalTasks.Count();
                }

                var messenger = DependencyService.Get<IMessenger>();
                messenger.LongMessage($"Ziel: {goalToEdit.Title} wurde erfolgreich bearbeitet.");

                await goalTaskRepository.SaveChangesAsync();

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();

                //TODO: Achievement unlockable for first editing of goal?

                //TODO: Alarms canceled properly?
                notificationQueueManager.CancelAlarms(goalToEdit);
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, goalToEdit,
                    new[] {goalToEdit.RequestCode, goalToEdit.RequestCode - 1, goalToEdit.RequestCode - 2}, username);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                    GoalStartDatePicker.Date.Month <= DateTime.Now.Month && !saving)
                {
                    GoalNotificationTimePicker.Time = new TimeSpan(hour + 1, 00, 00);
                    DependencyService.Get<IMessenger>().ShortMessage("Bitte wähle einen Zeitpunkt in der Zukunft aus.");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
                return null;
            }
        }

        private void InitializeGoalTasksComponent(Goal parent)
        {
            try
            {
                var goalTasks = AsyncHelpers.RunSync(() => goalTaskRepository.GetAllByParentAsync(goalToEdit));
                var goalTasksEnumerable = goalTasks as GoalTask[] ?? goalTasks.ToArray();
                if (goalTasksEnumerable.Any())
                {
                    foreach (var goalTask in goalTasksEnumerable)
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
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
            }
        }
    }
}