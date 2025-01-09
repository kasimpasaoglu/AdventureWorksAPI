using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface ICategoryService
{
    public List<CategoryDTO> GetAllCategories();
    public List<SubcategoryDTO> GetSubcategories(int categoryId);
}

public class CategoryService : ICategoryService
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public List<CategoryDTO> GetAllCategories()
    {
        var answer = _unitOfWork.Category.GetAll().Result.ToList();
        return _mapper.Map<List<CategoryDTO>>(answer);
    }

    public List<SubcategoryDTO> GetSubcategories(int categoryId)
    {
        var subcategories = _unitOfWork.SubCategory
            .Find<ProductSubcategory>(x => x.ProductCategoryId == categoryId)
            .Result
            .ToList();

        return _mapper.Map<List<SubcategoryDTO>>(subcategories);
    }
}