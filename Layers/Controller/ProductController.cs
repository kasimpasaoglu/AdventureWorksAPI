using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IMapper _mapper;
    public ProductController(IProductService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }


    /// <summary>
    /// Returns a list of products based on the given filters.
    /// </summary>
    /// <remarks>
    /// The `sortBy` parameter can take the following values:
    /// - `PriceAsc` : Sort by price in ascending order.
    /// - `PriceDesc` : Sort by price in descending order.
    /// - `NameAsc` : Sort by name in ascending order.
    /// - `NameDesc` : Sort by name in descending order.
    ///
    /// By default:
    /// - `pageSize` is set to `12`, indicating 12 products per page.
    /// - `pageNumber` is set to `1`, indicating the first page.
    ///
    /// Example request body:
    /// ```
    /// {
    ///   "ProductCategoryId": 1,
    ///   "ProductSubcategoryId": 2,
    ///   "MinPrice": 50.00,
    ///   "MaxPrice": 100.00,
    ///   "SelectedColors": ["Red", "Blue"],
    ///   "SortBy": "PriceAsc",
    ///   "PageSize": 10,
    ///   "PageNumber": 1
    /// }
    /// ```
    /// </remarks>
    /// <param name="filter">Filters to apply when fetching products.</param>
    /// <returns>A filtered list of products, or no content if no products match the filters.</returns>
    /// <response code="200">Returns the filtered product list.</response>
    /// <response code="204">No products match the given filters.</response>
    /// <response code="400">If the input model is invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductVM>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public IActionResult Post([FromBody] ProductFilterModel filter)
    {
        var answer = _service.GetProducts(filter);
        var productList = _mapper.Map<List<ProductVM>>(answer);
        if (productList != null && productList.Count > 0)
        {
            return Ok(productList);
        }
        return NoContent(); // Indicates no matching products found
    }

}