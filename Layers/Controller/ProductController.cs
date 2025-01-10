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
    ///     "ProductCategoryId" : 1,
    ///     "ProductSubCategoryId" : 2,
    ///     "MinPrice" : 2000.0,
    ///     "MaxPrice" : 2500.0,
    ///     "SelectedColors" : ["Red"],
    ///     "SortBy" : "PriceDesc",
    ///     "searchString" : "Road",
    ///     "PageSize" : 12,
    ///     "PageNumber" : 1
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
        return NoContent();
    }


    /// <summary>
    /// Retrieves a single product by its unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches the details of a product based on the provided product ID. 
    /// The response contains information such as product name, price, category, and other details.
    /// 
    /// Example request:
    /// ```
    /// GET /api/product/755
    /// ```
    /// </remarks>
    /// <param name="id">The unique identifier of the product to retrieve.</param>
    /// <returns>The product details, or no content if the product is not found.</returns>
    /// <response code="200">Returns the details of the product.</response>
    /// <response code="204">No product found with the given ID.</response>
    /// <response code="400">If the provided ID is invalid.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DetailedProductVM))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Get(int id)
    {
        var answer = _service.GetSingleProduct(id);
        if (answer != null)
        {
            return Ok(_mapper.Map<DetailedProductVM>(answer));
        }
        return NoContent();
    }

    /// <summary>
    /// Retrieves a list of the most recent products.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches a list of the most recently added products, sorted by their creation date.
    /// 
    /// - If the `count` parameter is provided and greater than 0, the specified number of products will be returned.
    /// - If no `count` parameter is provided or it is 0, a default of 12 products will be returned.
    /// 
    /// **Examples:**
    /// - `GET /api/products/recent` → Returns the 12 most recent products.
    /// - `GET /api/products/recent?count=5` → Returns the 5 most recent products.
    /// </remarks>
    /// <param name="count">Optional. The number of recent products to return. Defaults to 12 if not specified</param>
    /// <returns>
    /// A list of the most recent products.
    /// </returns>
    /// <response code="200">Returns the list of recent products.</response>
    /// <response code="400">If the `count` parameter is invalid.</response>
    [HttpGet("Recent")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductVM>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetRecent(int count)
    {
        var filter = new ProductFilterModel() { SortBy = "DateAsc" };
        _ = count > 0 ? filter.PageSize = count : filter.PageSize = 12;
        var answer = _service.GetProducts(filter);
        if (answer.Count() > 0)
        {
            return Ok(answer);
        }
        return NoContent();
    }
}