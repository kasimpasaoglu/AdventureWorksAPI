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
    /// Returns Product List
    /// </summary>
    /// <returns>List of Products</returns>
    [HttpGet]
    public IActionResult Get()
    {
        // Servisten tüm ürünleri getir
        var products = _service.GetProducts(null); // Filtre olmadan tüm ürünleri getir
        
        // Ürünleri ViewModel'e dönüştür
        var productList = _mapper.Map<List<ProductVM>>(products);
        
        // Eğer ürünler varsa döndür, yoksa NoContent döndür
        if (productList != null && productList.Count > 0)
        {
            return Ok(productList);
        }
        return NoContent(); // apinin başarılı bir şekilde çalıştığını ancak dönecek bir içerik olmadığını belirtir
    }


    /// <summary>
    /// Returns Product List by given Filters
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post([FromBody] ProductFilterModel filter)
    {
        var answer = _service.GetProducts(filter);
        var productList = _mapper.Map<List<ProductVM>>(answer);
        return Ok(productList);
    }
}