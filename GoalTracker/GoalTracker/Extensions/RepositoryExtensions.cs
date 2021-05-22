using System;
using System.Linq;
using System.Threading.Tasks;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;

namespace GoalTracker.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<int> GetNextNotificationId(this IGoalRepository goalRepository)
        {
            try
            {
                var goals = await goalRepository.GetAllAsync();

                var enumerable = goals.ToList();
                if (enumerable.Any())
                    return enumerable.Max(goal => goal.NotificationId) + 1;

                return 1;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return -1;
            }
        }

        public static async Task<int[]> GetNextRequestCodesForNotificationWithOptions(
            this IGoalRepository goalRepository)
        {
            try
            {
                var goals = await goalRepository.GetAllAsync();

                var startingRequestCode = 1;

                var enumerable = goals.ToList();
                if (enumerable.Any())
                    startingRequestCode = enumerable.Max(goal => goal.RequestCode) + 1;

                return new[] {startingRequestCode, startingRequestCode + 1, startingRequestCode + 2};
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return new[] {-1, -2, -3};
            }
        }
    }
}