using System.Linq.Expressions;
using AdventureWorksAPI.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAll();
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
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

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<List<T>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }


}