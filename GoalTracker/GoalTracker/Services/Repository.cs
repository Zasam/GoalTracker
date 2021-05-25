using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Services.Interface;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;

namespace GoalTracker.Services
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IGoalTrackerContext Context;

        protected Repository(IGoalTrackerContext context)
        {
            Context = context;
        }

        protected async Task<TEntity> GetAsync(int id)
        {
            try
            {
                return await Context.Set<TEntity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        protected async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await Context.Set<TEntity>().ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await Context.Set<TEntity>().Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return null;
            }
        }

        protected async Task AddAsync(TEntity entity)
        {
            try
            {
                await Context.Set<TEntity>().AddAsync(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await Context.Set<TEntity>().AddRangeAsync(entities);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected async Task RemoveAsync(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Remove(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                Context.Set<TEntity>().RemoveRange(entities);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task<bool> ExistsAsync(TEntity entity)
        {
            try
            {
                return await Context.Set<TEntity>().ContainsAsync(entity);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}