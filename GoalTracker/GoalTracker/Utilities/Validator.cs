using GoalTracker.Entities;

namespace GoalTracker.Utilities
{
    public static class Validator
    {
        //TODO: Expand validator?
        public static bool ValidateGoalInputs(Goal goal)
        {
            var valid = !string.IsNullOrWhiteSpace(goal.Title);
            return valid;
        }
    }
}