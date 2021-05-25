using System;
using System.Linq;
using GoalTracker.Extensions;
using GoalTracker.PlatformServices;
using GoalTracker.ViewModels.Interface;
using Microsoft.AppCenter.Crashes;
using Syncfusion.XForms.ProgressBar;
using Syncfusion.XForms.TextInputLayout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views.Main.Home.Goals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddGoalPage : ContentPage
    {
        public ISettingViewModel SettingViewModel { get; }

        private readonly IGoalViewModel goalViewModel;
        private int goalTaskCounter;
        private string[] goalTaskTitles;

        public AddGoalPage(IGoalViewModel goalViewModel, ISettingViewModel settingViewModel)
        {
            try
            {
                this.goalViewModel = goalViewModel;
                SettingViewModel = settingViewModel;
                goalViewModel.GoalHasDueDate = false;
                BindingContext = goalViewModel;

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
                ResetInputs();
                base.OnAppearing();
                AchievementStackLayout.InitializeAchievementAnimation();
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
                //TODO: Move validation to command in viewmodel!
                //var valid = ValidateInputs(newGoal);

                DependencyService.Get<IMessenger>()
                    .LongMessage("Neues Ziel erfolgreich hinzugefügt.");

                //TODO: how to identify if new achievement was unlocked? or is already unlocked?
                await AchievementStackLayout.StartAchievementUnlockedAnimation(AchievementLabel,
                    AchievementProgressBar, "Erfolg freigeschaltet: Dein erstes Ziel 🚀 erstellt" + Environment.NewLine + "Du hast dein erstes Ziel gesetzt, Super!");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void ResetInputs()
        {
            try
            {
                goalViewModel.GoalTitle = string.Empty;
                goalViewModel.GoalNotes = string.Empty;
                goalViewModel.GoalHasDueDate = false;
                goalViewModel.GoalNotificationTime = TimeSpan.FromHours(DateTime.Now.Hour);
                goalViewModel.GoalNotificationIntervalIndex = 3;
                goalViewModel.GoalImage = string.Empty;
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
                var text = GoalImageEntry.Text;

                // TODO: Implement check if a emoji was selected | All emojis equal to string.Length => 2?
                if (text.Length != 0 && text.Length != 2)
                {
                    goalViewModel.GoalImage = string.Empty;
                    DependencyService.Get<IMessenger>().ShortMessage("Bitte hinterlege ein Emoji als beschreibendes Bild für dein Ziel");
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
                taskTitleEntry.TextChanged += TaskTitleEntry_TextChanged;

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

        private void TaskTitleEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                GetTasks();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        // TODO: Implement tasks in viewmodel!
        private void GetTasks()
        {
            try
            {
                // TODO: Where to exactly call this method?
                goalTaskTitles = new string[goalTaskCounter];

                if (goalTaskCounter <= 0) return;
                for (var i = 0; i <= goalTaskCounter - 1; i++)
                {
                    var taskChildLayout = GoalTaskStackLayout.Children[i] as StackLayout;
                    var taskTitleTextInputLayout = taskChildLayout?.Children[0] as SfTextInputLayout;
                    var title = !(taskTitleTextInputLayout?.InputView is Entry taskTitleEntry) ? string.Empty : taskTitleEntry.Text;
                    goalTaskTitles[i] = title;
                }

                goalViewModel.GoalTaskTitles = goalTaskTitles.ToList();
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
                await Navigation.PopAsync(true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}