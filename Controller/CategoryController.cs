using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private ICategoryService _catService;
    private IMapper _mapper;
    public CategoryController(ICategoryService catService, IMapper mapper)
    {
        _catService = catService;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns All Available Categories with Category ID's and Names
    /// </summary>
    /// <returns>List of Categories</returns>
    [HttpGet]
    public IActionResult Get()
    {
        var answer = _catService.GetAllCategories();
        if (answer != null || answer.Count > 0)
        {
            var result = _mapper.Map<List<CategoryVM>>(answer);
            return Ok(result);
        }
        return NoContent();
    }

    /// <summary>
    /// Returns Subcategories for a Given Category ID
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of Subcategories</returns>
    [HttpGet("{categoryId}")]
    public IActionResult GetSubcategories(int categoryId)
    {
        var answer = _catService.GetSubcategoriesByCategoryId(categoryId);
        if (answer != null && answer.Count > 0)
        {
            var result = _mapper.Map<List<SubcategoryVM>>(answer);
            return Ok(result);
        }
        return NoContent();
    }
}