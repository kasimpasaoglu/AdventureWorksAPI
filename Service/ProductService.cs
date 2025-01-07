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

    List<ProductDTO> IProductService.GetProducts(ProductFilterModel filter)
    {
        var expression = ExpressionMaker.Make(filter);

        // var parameters = new[] { typeof(Product), typeof(int) };

        // object[]values = null;

        // DynamicExpressionParser.ParseLambda<Product, bool>(parameters, typeof(bool), expression, values);
        // var result = _unitOfWork.Product.Find(expression);
        // var resultDTO = _mapper.Map<List<ProductDTO>>(result);
        return null;
    }


    public ProductDTO GetSingleProduct(int productId)
    {
        throw new NotImplementedException();
    }
}