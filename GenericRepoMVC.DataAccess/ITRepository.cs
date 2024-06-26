using System.Linq.Expressions;

namespace GenericRepoMVC.DataAccess
{
    public interface ITRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
        Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<int> Delete(TEntity entity);
        Task<int> SaveAsync();
    }
}
