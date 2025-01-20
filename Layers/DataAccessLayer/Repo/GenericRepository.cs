using System.Linq.Expressions;
using AdventureWorksAPI.DataAccessLayer;
using AdventureWorksAPI.Models.DMO;
using Microsoft.EntityFrameworkCore;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<TResult>> Find<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector = null,
        bool distinct = false);
    Task<TResult> FindSingle<TResult>
    (
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        string[] includeProperties = null
    );
    Task<List<TResult>> FindWithProjection<TResult>
    (
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        string[]? includeProperties = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? pagination = null
    );
    Task<List<T>> GetAll();
    Task<T> AddAsync(T entity);
    Task RemoveAsync(T entity);
    Task UpdateAsync(T entity);
}
////

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AdventureWorksContext _context;
    private readonly DbSet<T> _dbSet;
    public GenericRepository(AdventureWorksContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<TResult>> Find<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector = null,
        bool distinct = false)
    {
        if (selector != null)
        {
            return await _dbSet.Where(predicate).Select(selector).Distinct().ToListAsync();
        }

        // Selector yoksa T türünde dönüşüm yap
        return await _dbSet.Where(predicate).ToListAsync() as IEnumerable<TResult>;
    }

    public async Task<TResult> FindSingle<TResult>
    (
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        string[] includeProperties = null
    )
    {
        IQueryable<T> query = _dbSet;
        if (includeProperties != null && includeProperties.Length > 0)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public async Task<List<TResult>> FindWithProjection<TResult>
    (
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        string[]? includeProperties = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? pagination = null
    )
    {
        IQueryable<T> query = _dbSet;

        // include varsa
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        // filtre varsa
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // Sıralama varsa
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Sayfalama varsa
        if (pagination != null)
        {
            query = pagination(query);
        }


        return await query.Select(selector).ToListAsync();

    }

    public async Task<List<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }




}