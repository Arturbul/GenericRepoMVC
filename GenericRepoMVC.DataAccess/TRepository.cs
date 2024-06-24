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
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
               query = orderBy(query);
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>>? filter = null)
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

        public async Task<TEntity> Update(TEntity entity)
        {
            var updated = _context.Entry(entity);
            updated.State = EntityState.Modified;

            await this.SaveAsync();
            return updated.Entity;
        }
        public async Task<object> Delete(TEntity entity)
        {
            var deleted = _context.Entry(entity);
            deleted.State = EntityState.Deleted;

            return await this.SaveAsync(); //changed entities count
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
