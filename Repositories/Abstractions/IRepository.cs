using BusinessObject.Entities;
using System.Linq.Expressions;

namespace Repositories.Abstractions
{
    public interface IRepository<T, TKey> where T : class, IEntity<TKey>
    {
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> items, CancellationToken ct = default);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync(CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
        IQueryable<T> Query();
    }
}
