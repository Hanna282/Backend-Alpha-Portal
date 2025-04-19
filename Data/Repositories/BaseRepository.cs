using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<bool> CreateAsync(TEntity entity);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);
        Task<IEnumerable<TEntity>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes);
        Task<bool> UpdateAsync(TEntity entity);
    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DataContext _context;
        protected readonly DbSet<TEntity> _table;

        public BaseRepository(DataContext context)
        {
            _context = context;
            _table = _context.Set<TEntity>();
        }

        public virtual async Task<bool> CreateAsync(TEntity entity)
        {
            if (entity == null)
                return false;

            try
            {
                _table.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            if (expression == null)
                return false;

            try
            {
                var entity = await _table.FirstOrDefaultAsync(expression);
                if (entity == null)
                    return false;

                _table.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
        {
            if (expression == null)
                return false;

            try
            {
                var exists = await _table.AnyAsync(expression);
                return exists;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            bool orderByDescending = false,
            Expression<Func<TEntity, object>>? sortBy = null,
            Expression<Func<TEntity, bool>>? filterBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                IQueryable<TEntity> query = _table;

                if (filterBy != null)
                    query = query.Where(filterBy);

                if (includes != null && includes.Length != 0)
                    foreach (var include in includes)
                        query = query.Include(include);

                if (sortBy != null)
                    query = orderByDescending
                        ? query.OrderByDescending(sortBy)
                        : query.OrderBy(sortBy);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return Enumerable.Empty<TEntity>();
            }
        }

        public virtual async Task<TEntity?> GetAsync(
            Expression<Func<TEntity, bool>> findBy,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                IQueryable<TEntity> query = _table;

                if (includes != null && includes.Length != 0)
                    foreach (var include in includes)
                        query = query.Include(include);

                var entity = await query.FirstOrDefaultAsync(findBy);
                return entity ?? null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return null;
            }
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                return false;

            try
            {
                _table.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
