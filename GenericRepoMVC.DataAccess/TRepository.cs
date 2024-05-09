using GenericRepoMVC.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GenericRepoMVC.DataAccess
{
    public class TRepository<TEntity> : ITRepository<TEntity>
        where TEntity : class
    {
        private readonly GenericRepoMVCContext _context;
        public TRepository(GenericRepoMVCContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToArrayAsync();
            }
            return await query.ToArrayAsync();
        }

        public async Task<TEntity?> Get(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<TEntity> Create(TEntity entity)
        {
            var added = _context.Entry(entity);
            added.State = EntityState.Added;

            await this.SaveAsync();
            return added.Entity;
        }

        public Task<TEntity> Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task<object> Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<object> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {

                throw new Exception(e.Message, e);
            }
        }
    }
}
