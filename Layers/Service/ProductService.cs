using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface IProductService
{
    public List<ProductDTO> GetProducts(ProductFilterModel filter);
    public DetailedProductDTO GetSingleProduct(int productId);
}

public class ProductService : IProductService
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public List<ProductDTO> GetProducts(ProductFilterModel filter)
    {
        // tablo joinleme
        var includes = Constants.GetSimpleIncludes();

        // where kosullari expression olusturma
        var predicate = ExpressionHelper.BuildWhereExpression(filter);

        // siralama icin orderby expression
        var orderByExp = ExpressionHelper.BuildOrderByExpression(filter.SortBy);

        // sayfalama icin(Take, Skip) expression
        var paginationExp = ExpressionHelper.BuildPaginationExpression(filter.PageNumber, filter.PageSize);


        // projeksiyon(select) olusturma 
        Expression<Func<Product, ProductDTO>> selector = Constants.GetSimpleSelectors();

        return _unitOfWork.Product.FindWithProjection<ProductDTO>(selector, predicate, includes, orderByExp, paginationExp).Result.ToList();
    }

    public DetailedProductDTO GetSingleProduct(int productId)
    {
        var includes = Constants.GetDetailedIncludes();

        var combinedExpression = ExpressionHelper.CombineExpressions<Product>(
            x => x.ProductId == productId,
            x => x.StandardCost > 0,
            x => x.ProductSubcategoryId != null
        );

        Expression<Func<Product, DetailedProductDTO>> selector = Constants.GetDetailedSelectors();


        return _unitOfWork.Product.FindSingle<DetailedProductDTO>(combinedExpression, selector, includes).Result;
    }

}