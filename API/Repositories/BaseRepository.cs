using IceAndStone.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using IceAndStone.API.Repositories.Abstractions;

namespace IceAndStone.API.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext DbContext;
        protected readonly DbSet<TEntity> Set;

        public BaseRepository(AppDbContext dbContext)
        {
            DbContext = dbContext;
            Set = dbContext.Set<TEntity>();
        }

        public virtual Task<TEntity?> GetByIdAsync(object id, CancellationToken ct)
            => Set.FindAsync(new object?[] { id }, ct).AsTask();

        public virtual async Task AddAsync(TEntity entity, CancellationToken ct)
            => await Set.AddAsync(entity, ct);

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct)
            => await Set.AddRangeAsync(entities, ct);

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
            => Set.AnyAsync(predicate, ct);

        public virtual Task<int> SaveChangesAsync(CancellationToken ct)
            => DbContext.SaveChangesAsync(ct);

        public IQueryable<TEntity> Query() => Set.AsQueryable();
    }
}
