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

        // var result = _unitOfWork.Product.Find(expression);
        return null;
    }


    public ProductDTO GetSingleProduct(int productId)
    {
        throw new NotImplementedException();
    }
}