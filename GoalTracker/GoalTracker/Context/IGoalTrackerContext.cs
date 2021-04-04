using System.Threading;
using System.Threading.Tasks;
using GoalTracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Context
{
    public interface IGoalTrackerContext
    {
        DbSet<Goal> Goals { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Achievement> Achievements { get; set; }
        DbSet<GoalTask> GoalTasks { get; set; }
        DbSet<GoalAppointment> GoalAppointments { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}