using System;
using GoalTracker.Entities;
using GoalTracker.Extensions;
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
    public partial class AddGoalPage : ContentPage
    {
        private readonly string username;
        private readonly IGoalViewModel goalViewModel;
        private readonly ISettingViewModel settingViewModel;
        private bool contentLoaded;
        private int goalTaskCounter;
        private bool saving;

        public AddGoalPage(IGoalViewModel goalViewModel, ISettingViewModel settingViewModel, string username)
        {
            this.username = username;
            this.goalViewModel = goalViewModel;
            this.settingViewModel = settingViewModel;

            contentLoaded = false;
            goalViewModel.GoalHasDueDate = false;
            BindingContext = goalViewModel;

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
                contentLoaded = true;
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
                saving = true;

                var newGoal = new Goal(goalViewModel.GoalTitle, goalViewModel.GoalNotes, goalViewModel.GoalStartDate, goalViewModel.GoalHasDueDate, goalViewModel.GoalEndDate, goalViewModel.GoalNotificationInterval, goalViewModel.GoalNotificationTime,
                    goalViewModel.GoalImage);

                var valid = ValidateInputs(newGoal);
                if (!valid)
                    return;

                var goalTasks = GetTasks(newGoal);
                var goal = await goalViewModel.AddGoalAsync(newGoal, goalTasks, username);

                if (goal != null)
                {
                    DependencyService.Get<IMessenger>()
                        .LongMessage($"Neues Ziel erfolgreich hinzugefügt: {newGoal.Title}.");

                    var unlockableAchievement = await settingViewModel.GetAchievementAsync("GOALADD");

                    if (unlockableAchievement != null)
                    {
                        var unlocked = await settingViewModel.UnlockAchievementAsync("GOALADD");

                        if (unlocked)
                            await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel,
                                AchievementProgressBar, unlockableAchievement.Title);
                    }

                    await Navigation.PopAsync(true);
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

        private void ResetInputs()
        {
            try
            {
                GoalTitleEntry.Text = string.Empty;
                GoalNotesEditor.Text = string.Empty;
                GoalStartDatePicker.Date = goalViewModel.GoalMinimumStartDate;
                GoalHasDueDateCheckBox.IsChecked = false;
                GoalEndDatePicker.Date = goalViewModel.GoalMinimumEndDate;
                GoalNotificationTimePicker.Time = TimeSpan.FromHours(DateTime.Now.Hour);
                GoalNotificationIntervalPicker.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
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

        private void GoalImageEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = GoalImageEntry.Text;
            if (text.Length != 2)
            {
                // TODO: Implement check if a emoji was selected
                GoalImageEntry.Text = string.Empty;
                DependencyService.Get<IMessenger>()
                    .ShortMessage("Bitte hinterlege nur ein Emoji als beschreibendes Bild");
            }
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
    }
}