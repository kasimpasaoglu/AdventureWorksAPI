using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class ColorController : ControllerBase
{
    private IColorService _colorService;
    public ColorController(IColorService colorService)
    {
        _colorService = colorService;
    }


    /// <summary>
    /// Retrieves a list of colors based on category and subcategory filters.
    /// </summary>
    /// <remarks>
    /// This endpoint allows fetching colors based on optional category (`categoryId`) and subcategory (`subCategoryId`) filters. 
    /// 
    /// - If no parameters are provided, all available colors will be returned.
    /// - If only `catId` is provided, colors belonging to the specified category will be returned.
    /// - If both `catId` and `subCatId` are provided, colors belonging to the specified category and subcategory will be returned.
    ///
    /// **Examples:**
    /// - `GET /api/colors` → Returns all colors.
    /// - `GET /api/colors/1` → Returns colors for category ID 1.
    /// - `GET /api/colors/1/2` → Returns colors for category ID 1 and subcategory ID 2.
    /// </remarks>
    /// <param name="categoryId">Optional category ID to filter colors by.</param>
    /// <param name="subCategoryId">Optional subcategory ID to further filter colors by.</param>
    /// <returns>
    /// A list of colors matching the specified filters, or no content if no colors match.
    /// </returns>
    /// <response code="200">Returns the list of matching colors.</response>
    /// <response code="204">No colors match the specified filters.</response>
    [HttpGet("{categoryId:int?}/{subCategoryId:int?}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Get(int? categoryId = null, int? subCategoryId = null)
    {
        var result = _colorService.GetColors(categoryId, subCategoryId);
        if (result.Length > 0)
        {
            return Ok(result);
        }
        return NoContent();
    }

}