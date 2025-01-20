using AdventureWorksAPI.DataAccessLayer;
using AdventureWorksAPI.Models.DMO;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Product> Product { get; }
    IGenericRepository<ProductCategory> Category { get; }
    IGenericRepository<ProductSubcategory> SubCategory { get; }
    IGenericRepository<Person> Person { get; }
    IGenericRepository<Password> Password { get; }
    IGenericRepository<EmailAddress> EmailAddress { get; }
    IGenericRepository<BusinessEntity> BusinessEntity { get; }
    IGenericRepository<Address> Address { get; }
    IGenericRepository<BusinessEntityAddress> BusinessEntityAddress { get; }
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private AdventureWorksContext _context;

    public IGenericRepository<Product> Product { get; private set; }
    public IGenericRepository<ProductCategory> Category { get; private set; }
    public IGenericRepository<ProductSubcategory> SubCategory { get; private set; }
    public IGenericRepository<Person> Person { get; private set; }
    public IGenericRepository<Password> Password { get; private set; }
    public IGenericRepository<EmailAddress> EmailAddress { get; private set; }
    public IGenericRepository<BusinessEntity> BusinessEntity { get; private set; }
    public IGenericRepository<Address> Address { get; private set; }
    public IGenericRepository<BusinessEntityAddress> BusinessEntityAddress { get; private set; }

    public UnitOfWork(AdventureWorksContext context)
    {
        _context = context;
        Product = new GenericRepository<Product>(_context);
        Category = new GenericRepository<ProductCategory>(_context);
        SubCategory = new GenericRepository<ProductSubcategory>(_context);
        Person = new GenericRepository<Person>(_context);
        Password = new GenericRepository<Password>(_context);
        EmailAddress = new GenericRepository<EmailAddress>(_context);
        BusinessEntity = new GenericRepository<BusinessEntity>(_context);
        Address = new GenericRepository<Address>(_context);
        BusinessEntityAddress = new GenericRepository<BusinessEntityAddress>(_context);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.RollbackAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
