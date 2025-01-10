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
    /// Retrieves all available categories with their IDs and names.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all categories available in the system. Each category includes its unique ID and name.
    /// 
    /// **Example:**
    /// - `GET /api/categories` → Returns all categories.
    /// </remarks>
    /// <returns>
    /// A list of categories, each containing a unique ID and name.
    /// </returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="204">No categories available.</response>
    /// <response code="400">Bad request if the request is invalid.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryVM>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// Retrieves subcategories for a given category ID.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches all subcategories belonging to a specific category, identified by the provided category ID.
    /// 
    /// **Example:**
    /// - `GET /api/categories/1` → Returns all subcategories under category ID 1.
    /// </remarks>
    /// <param name="categoryId">The unique identifier of the category whose subcategories are to be retrieved.</param>
    /// <returns>
    /// A list of subcategories under the specified category ID.
    /// </returns>
    /// <response code="200">Returns the list of subcategories for the given category ID.</response>
    /// <response code="204">No subcategories available for the given category ID.</response>
    /// <response code="400">Bad request if the category ID is invalid.</response>
    [HttpGet("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubcategoryVM>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetSubcategories(int categoryId)
    {
        var answer = _catService.GetSubcategories(categoryId);
        if (answer != null && answer.Count > 0)
        {
            var result = _mapper.Map<List<SubcategoryVM>>(answer);
            return Ok(result);
        }
        return NoContent();
    }
}