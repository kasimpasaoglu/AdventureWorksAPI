using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private ICartService _cartService;
    private IMapper _mapper;
    public CartController(ICartService cartService, IMapper mapper)
    {
        _cartService = cartService;
        _mapper = mapper;
    }


    // [Authorize]
    // [HttpGet]
    // public IActionResult Get()
    // {

    // }


    // [Authorize]
    // [HttpPost]
    // public IActionResult Post()
    // {

    // }


    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] InsertVM item)
    {
        try
        {
            var businessEntityId = Token.GetBusinessEntityId(User);

            var isOk = await _cartService.InsertNewItemAsync(item, businessEntityId);
            if (isOk)
            {
                return Ok(new { Message = "Item added successfully." });
            }
            return StatusCode(500, new { Error = "An unexpected error occurred." });
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message });
        }

    }


    // [Authorize]
    // [HttpDelete]
    // public IActionResult Delete()
    // {

    // }
}
