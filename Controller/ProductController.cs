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

    [HttpPost]
    public IActionResult Post([FromBody] ProductFilterModel filter)
    {
        var answer = _service.GetProducts(filter);
        var productList = _mapper.Map<List<ProductVM>>(answer);
        return Ok(productList);
    }
}