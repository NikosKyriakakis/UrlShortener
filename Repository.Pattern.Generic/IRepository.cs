using System.Linq.Expressions;

namespace Repository.Pattern.Generic
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<IReadOnlyCollection<TEntity>> GetAllAsync();
        Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);
        Task PostAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
    }
}
