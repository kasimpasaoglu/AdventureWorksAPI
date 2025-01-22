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


    /// <summary>
    /// Retrieves the shopping cart items for the authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches the shopping cart items associated with the currently authenticated user.
    /// It returns a summary of the cart including total price and item count, along with detailed information
    /// about each item in the cart.
    /// 
    /// **Authorization is required.**
    /// 
    /// </remarks>
    /// <returns>
    /// - Returns <see cref="ShoppingCartVM"/> with cart details and items if found.
    /// - Returns HTTP 204 if the cart is empty or no items are found.
    /// - Returns HTTP 500 if an unexpected error occurs.
    /// </returns>
    /// <response code="200">Returns the shopping cart details and items.</response>
    /// <response code="204">No content available, cart is empty.</response>
    /// <response code="500">Unexpected server error occurred.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCartVM))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var bussinessEntityId = Token.GetBusinessEntityId(User);
            var dto = await _cartService.GetCartItems(bussinessEntityId);

            if (dto == null)
            {
                return NoContent();
            }

            return Ok(_mapper.Map<ShoppingCartVM>(dto));

        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
        }
    }



    /// <summary>
    /// Adds or updates an item in the user's shopping cart.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the user to add a new item to their shopping cart or increasae the quantity if the item already exists.
    /// The user's identity is determined from the token in the request headers.
    /// 
    /// **Authorization is required.**
    /// </remarks>
    /// <param name="item">The shopping cart item details, including ProductId and Quantity.</param>
    /// <returns>A success message if the item is added/updated, or an error message if the operation fails.</returns>
    /// <response code="200">Item added/updated successfully.</response>
    /// <response code="404">User identification failed or other user-specific error.</response>
    /// <response code="500">Unexpected error occurred during the operation.</response>
    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put([FromBody] CartItemVM item)
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

    /// <summary>
    /// Removes or decreases the quantity of an item in the user's shopping cart.
    /// </summary>
    /// <remarks>
    /// This endpoint allows the user to decrease the quantity of an item in their shopping cart.
    /// If the quantity reaches zero, the item is completely removed from the cart.
    /// The user's identity is determined from the token in the request headers.
    /// 
    /// **Authorization is required.**
    /// </remarks>
    /// <param name="item">The shopping cart item details, including ProductId and Quantity.</param>
    /// <returns>A success message if the item is updated/removed, or an error message if the operation fails.</returns>
    /// <response code="200">Item updated/removed successfully.</response>
    /// <response code="404">User identification failed or other user-specific error.</response>
    /// <response code="500">Unexpected error occurred during the operation.</response>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromBody] CartItemVM item)
    {
        try
        {
            var businessEntityId = Token.GetBusinessEntityId(User);

            var isOk = await _cartService.DeleteItemAsync(item, businessEntityId);

            if (isOk)
            {
                return Ok(new { Message = "Item deleted successfully." });
            }
            return StatusCode(500, new { Error = "An unexpected error occurred." });
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}
