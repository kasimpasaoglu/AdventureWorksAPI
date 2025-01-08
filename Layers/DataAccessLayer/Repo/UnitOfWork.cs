using AdventureWorksAPI.DataAccessLayer;
using AdventureWorksAPI.Models.DMO;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Product> Product { get; }
    IGenericRepository<ProductCategory> Category { get; }
    IGenericRepository<ProductSubcategory> SubCategory { get; }

}

public class UnitOfWork : IUnitOfWork
{
    private AdventureWorksContext _context;


    public IGenericRepository<Product> Product { get; }
    public IGenericRepository<ProductCategory> Category { get; }
    public IGenericRepository<ProductSubcategory> SubCategory { get; }


    public UnitOfWork(AdventureWorksContext context)
    {
        _context = context;
        Product = new GenericRepository<Product>(_context);
        Category = new GenericRepository<ProductCategory>(_context);
        SubCategory = new GenericRepository<ProductSubcategory>(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}