using GoalTracker.Context;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.UnitTest
{
    public class BusinessLogicTest
    {
        protected DbContextOptions<GoalTrackerContext> ContextOptions { get; }

        protected BusinessLogicTest(DbContextOptions<GoalTrackerContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        private void Seed()
        {
            using var context = new GoalTrackerContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Add initial values to database for testing
        }
    }
}