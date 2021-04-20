using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GoalTracker.Services.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //protected Task<TEntity> GetAsync(int id);
        //protected Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        //protected Task AddAsync(TEntity entity);
        //protected Task AddRangeAsync(IEnumerable<TEntity> entities);
        //protected Task RemoveAsync(TEntity entity);
        //protected Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task<bool> ExistsAsync(TEntity entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}