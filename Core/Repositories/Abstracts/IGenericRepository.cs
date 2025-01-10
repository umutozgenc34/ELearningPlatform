using Core.Models;
using System.Linq.Expressions;

namespace Core.Repositories.Abstracts;

public interface IGenericRepository<TEntity, in TId> where TEntity : BaseEntity<TId>, new()
{
    IQueryable<TEntity> GetAll();
    ValueTask<TEntity?> GetByIdAsync(TId id);
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? predicate = null);
    ValueTask AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
