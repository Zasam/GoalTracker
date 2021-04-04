using System;
using System.Linq;
using GoalTracker.Entities;
using GoalTracker.Extensions;
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
    public partial class AddGoalPage : ContentPage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly IGoalAppointmentRepository goalAppointmentRepository;
        private readonly IGoalRepository goalRepository;
        private readonly IGoalTaskRepository goalTaskRepository;
        private readonly string username;
        private readonly IGoalViewModel viewModel;
        private int goalTaskCounter;
        private bool saving;

        public AddGoalPage(IGoalViewModel viewModel, IGoalRepository goalRepository,
            IGoalAppointmentRepository goalAppointmentRepository, IAchievementRepository achievementRepository,
            IGoalTaskRepository goalTaskRepository,
            string username)
        {
            this.username = username;
            this.viewModel = viewModel;
            this.goalRepository = goalRepository;
            this.goalAppointmentRepository = goalAppointmentRepository;
            this.achievementRepository = achievementRepository;
            this.goalTaskRepository = goalTaskRepository;

            viewModel.GoalHasDueDate = false;
            BindingContext = viewModel;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            try
            {
                saving = false;
                ResetInputs();
                base.OnAppearing();
                AchievementStackLayout.InitializeAchievementAnimation();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
                Crashes.TrackError(ex);
            }
        }

        private async void SaveGoalButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                saving = true;
                var goalNotificationId = await goalRepository.GetNextNotificationId();
                var goalRequestCodes = await goalRepository.GetNextRequestCodesForNotificationWithOptions();
                var newGoal = new Goal(viewModel.GoalTitle, viewModel.GoalNotes, viewModel.GoalStartDate,
                    viewModel.GoalHasDueDate, viewModel.GoalEndDate, viewModel.GoalNotificationInterval,
                    viewModel.GoalNotificationTime, goalNotificationId, goalRequestCodes.Max());

                var valid = ValidateInputs(newGoal);
                if (!valid)
                    return;

                await goalRepository.AddAsync(newGoal);

                var goalAppointments = newGoal.GetAppointments();
                await goalAppointmentRepository.AddRangeAsync(goalAppointments);

                var goalTasks = GetTasks(newGoal);
                if (goalTasks != null && goalTasks.Any())
                {
                    foreach (var goalTask in goalTasks)
                        await goalTaskRepository.AddAsync(goalTask);

                    //goalTaskRepository.AddRange(goalTasks);
                    newGoal.GoalTaskCount = goalTasks.Count();
                }

                await goalRepository.SaveChangesAsync();

                var messenger = DependencyService.Get<IMessenger>();
                messenger.LongMessage($"Neues Ziel erfolgreich hinzugefügt: {newGoal.Title}.");

                var notificationQueueManager = DependencyService.Get<INotificationQueueManager>();
                notificationQueueManager.QueueGoalNotificationBroadcast(goalRepository, newGoal, goalRequestCodes,
                    username);

                var unlockableAchievements = await
                    achievementRepository.FindAsync(a => a.InternalTag == "GOALADD");
                var unlockableAchievement = unlockableAchievements.FirstOrDefault();
                if (unlockableAchievement != null)
                {
                    var firstUnlock = unlockableAchievement.Unlock();
                    if (firstUnlock)
                    {
                        await achievementRepository.SaveChangesAsync();
                        await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel,
                            AchievementProgressBar, unlockableAchievement.Title);
                        await Navigation.PopAsync(true);
                    }
                    else
                    {
                        await Navigation.PopAsync(true);
                    }
                }
                else
                {
                    await Navigation.PopAsync(true);
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
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

        private void ResetInputs()
        {
            try
            {
                GoalTitleEntry.Text = string.Empty;
                GoalNotesEditor.Text = string.Empty;
                GoalStartDatePicker.Date = viewModel.GoalMinimumStartDate;
                GoalHasDueDateCheckBox.IsChecked = false;
                GoalEndDatePicker.Date = viewModel.GoalMinimumEndDate;
                GoalNotificationTimePicker.Time = TimeSpan.FromHours(DateTime.Now.Hour);
                GoalNotificationIntervalPicker.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessenger>()
                    .LongMessage("Es ist wohl etwas schief gelaufen. Ein Fehlerbericht wurde gesendet.");
                Crashes.TrackError(ex);
            }
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
    }
}