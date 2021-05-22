using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GoalTracker.Context;
using GoalTracker.Services.Interface;
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
            return await Context.Set<TEntity>().FindAsync(id);
        }

        protected async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        protected async Task AddAsync(TEntity entity)
        {
            await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        protected async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();
        }

        protected async Task RemoveAsync(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            await Context.SaveChangesAsync();
        }

        protected async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            await Context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(TEntity entity)
        {
            return await Context.Set<TEntity>().ContainsAsync(entity);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}