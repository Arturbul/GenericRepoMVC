using System.Linq.Expressions;

namespace GenericRepoMVC.Servicies
{
    public interface ITServicies<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
        Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity> CreateOrUpdate(TEntity entity);
        Task<int> Delete(TEntity entity);
    }
}
