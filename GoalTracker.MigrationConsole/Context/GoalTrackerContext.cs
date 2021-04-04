using System;
using GoalTracker.Entities;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Context
{
    public sealed class GoalTrackerContext : DbContext, IGoalTrackerContext
    {
        public GoalTrackerContext()
        {
            // iOS Requirement: SQLitePCL.Batteries_V2.Init();
            Database.Migrate();
        }

        public GoalTrackerContext(DbContextOptions contextOptions)
            : base(contextOptions)
        {
            Database.Migrate();
        }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<GoalAppointment> GoalAppointments { get; set; }
        public DbSet<GoalTask> GoalTasks { get; set; }

        public void Commit()
        {
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                //string dbPath = Path.Combine(FileSystem.AppDataDirectory, $"{nameof(GoalTracker)}.db3");
                var dbPath = "GoalTracker.db3";
                optionsBuilder.UseSqlite($"Filename={dbPath}");
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