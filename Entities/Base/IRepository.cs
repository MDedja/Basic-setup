using System.Linq.Expressions;

namespace Entities.Base;

public interface IRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Returns the entity corresponding to the given ID, or default if not found.
    /// </summary>
    /// 
    Task<TEntity> GetByIdAsync(Guid id);

    bool Exists(Guid id);

    Task<List<TEntity>> GetAllAsync(bool readOnly = false);

    Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, bool readOnly = false);

    Task<List<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, bool readOnly = false);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(List<TEntity> entities);

    Task UpdateAsync(TEntity entity);

    Task UpdateAndActivateAsync(TEntity entity, Guid id);

    Task UpdateRangeAsync(List<TEntity> entities);

    Task RemoveAsync(TEntity entity);

    Task RemoveRangeAsync(List<TEntity> entities);
}