using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoalTracker.Entities;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace GoalTracker.Context
{
    public sealed class GoalTrackerContext : DbContext, IGoalTrackerContext
    {
        public DbSet<Goal> Goals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GoalTask> GoalTasks { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<GoalAppointment> GoalAppointments { get; set; }

        public GoalTrackerContext()
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public GoalTrackerContext(DbContextOptions contextOptions)
            : base(contextOptions)
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified));

                foreach (var entityEntry in entries)
                {
                    ((BaseEntity) entityEntry.Entity).UpdateDate = DateTime.Now;

                    if (entityEntry.State == EntityState.Added)
                        ((BaseEntity) entityEntry.Entity).CreateDate = DateTime.Now;
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return -1;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                // Do not configure database provider when context options are already configured, e.g. by unit tests
                if (!optionsBuilder.IsConfigured)
                {
                    var dbPath = Path.Combine(FileSystem.AppDataDirectory,
                        $"{nameof(GoalTracker)}-Development_22042021.db3");

                    optionsBuilder.UseSqlite($"Filename={dbPath}");
                }

                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                // Model keys
                modelBuilder.Entity<Goal>().HasKey(g => g.Id);
                modelBuilder.Entity<User>().HasKey(us => us.Id);
                modelBuilder.Entity<Achievement>().HasKey(a => a.Id);
                modelBuilder.Entity<GoalAppointment>().HasKey(ga => ga.Id);
                modelBuilder.Entity<GoalTask>().HasKey(gt => gt.Id);

                // Model relationships
                modelBuilder.Entity<GoalTask>()
                    .HasOne(gt => gt.Goal)
                    .WithMany(g => g.GoalTasks)
                    .HasForeignKey(gt => gt.GoalId);

                modelBuilder.Entity<GoalAppointment>()
                    .HasOne(gd => gd.Goal)
                    .WithMany(g => g.GoalAppointments)
                    .HasForeignKey(gd => gd.GoalId);

                modelBuilder.Entity<Achievement>()
                    .HasOne(a => a.User)
                    .WithMany(u => u.Achievements)
                    .HasForeignKey(a => a.UserId);

                base.OnModelCreating(modelBuilder);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}