using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface IProductService
{
    public List<ProductDTO> GetProducts(ProductFilterModel filter);
    public ProductDTO GetSingleProduct(int productId);
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
        var includes = new[]
        {
            "ProductProductPhotos.ProductPhoto",
            "ProductModel.ProductModelProductDescriptionCultures.ProductDescription",
            "ProductSubcategory.ProductCategory"
        };

        // expression olusturma
        var predicate = ExpressionBuilder.BuildWhereExpression(filter);

        // siralama icin expression
        var orderByExp = ExpressionBuilder.BuildOrderByExpression(filter.SortBy);

        // sayfalama icin expression
        var paginationExp = ExpressionBuilder.BuildPaginationExpression(filter.PageNumber, filter.PageSize);


        // projeksiyon olusturma 
        Expression<Func<Product, ProductDTO>> selector = p => new ProductDTO
        {
            ProductId = p.ProductId,
            Name = p.Name,
            ListPrice = p.ListPrice,
            StandardCost = p.StandardCost,
            Color = p.Color,
            ProductCategoryId = p.ProductSubcategory.ProductCategoryId,
            ProductSubcategoryId = p.ProductSubcategoryId,
            Description = p.ProductModel.ProductModelProductDescriptionCultures.FirstOrDefault().ProductDescription.Description,
            LargePhoto = p.ProductProductPhotos.FirstOrDefault().ProductPhoto.LargePhoto
        };

        return _unitOfWork.Product.FindWithProjection<ProductDTO>(selector, predicate, includes, orderByExp, paginationExp).Result.ToList();
    }


    public ProductDTO GetSingleProduct(int productId)
    {
        throw new NotImplementedException();
    }
}