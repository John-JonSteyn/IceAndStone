using System.Linq.Expressions;

namespace IceAndStone.API.Repositories.Abstractions
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(object id, CancellationToken ct);
        Task AddAsync(TEntity entity, CancellationToken ct);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
        IQueryable<TEntity> Query();
    }
}
