

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Registers a new user with the given information.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new user in the system using the provided registration data. 
    /// It validates the input, hashes the password with a unique salt, and stores the data across multiple related tables such as `Person`, `EmailAddress`, `Password`, and `Address`.
    ///
    /// This operation is transactional, ensuring that either all data is saved successfully or nothing is saved in case of an error.
    /// </remarks>
    /// <param name="dto">The user registration data.</param>
    /// <returns>A success message if the user is registered successfully.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Validation failed or input data is invalid.</response>
    /// <response code="500">An internal server error occurred while processing the request.</response>
    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterVM model)
    {
        var dto = _mapper.Map<RegisterDTO>(model);
        try
        {
            await _userService.RegisterUserAsync(dto);
            return Ok(new { Message = "User Registered Succesfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message, Details = ex.InnerException?.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An unexpected error occurred", Detals = ex.Message });
        }

    }

    /// <summary>
    /// Retrieves a list of all states (StateProvince) with their IDs and names.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches all states from the database and returns their unique identifiers (StateProvinceId) and names. 
    /// The data is primarily used for populating dropdowns or selection fields in the user registration process.
    /// 
    /// Example request:
    ///
    /// </remarks>
    /// <returns>
    /// Returns a list of states in a `StateVM` format if any exist. 
    /// If no states are found, returns a 404 Not Found response.
    /// </returns>
    /// <response code="200">Returns the list of states with their IDs and names.</response>
    /// <response code="404">No states found in the database.</response>
    [HttpGet("States")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StateVM>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetStates()
    {
        var dtoList = _userService.GetAllStates();
        if (dtoList.Count > 0)
        {
            return Ok(_mapper.Map<List<StateVM>>(dtoList));
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Retrieves a list of all address types with their IDs and names.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches all address types from the database and returns their unique identifiers (`AddressTypeId`) and names. 
    /// The data is used for populating dropdowns or selection fields during user registration, allowing the user to select the type of address (e.g., Home, Billing, Shipping).
    /// 
    /// </remarks>
    /// <returns>
    /// Returns a list of address types in `AddressTypeVM` format if any exist. 
    /// If no address types are found, returns a 404 Not Found response.
    /// </returns>
    /// <response code="200">Returns the list of address types with their IDs and names.</response>
    /// <response code="404">No address types found in the database.</response>
    [HttpGet("AddressTypes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AddressTypeVM>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAddressTypes()
    {
        var dtoList = _userService.GetAddressTypes();
        if (dtoList.Count > 0)
        {
            return Ok(_mapper.Map<List<AddressTypeVM>>(dtoList));
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Validates a user login attempt.
    /// </summary>
    /// <param name="Login">The login data (email and password).</param>
    /// <returns>Returns detailed messages for each validation step.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var result = await _userService.ValidateUserAsync(login);

        if (!result.IsSuccessful)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message, result.BusinessEntityId, result.Token });
    }

    /// <summary>
    /// Deletes a user's account based on the provided BusinessEntityId.
    /// </summary>
    /// <remarks>
    /// This endpoint allows an authenticated user to delete their own account. 
    /// The `BusinessEntityId` provided in the request URL must match the authenticated user's ID. 
    /// 
    /// **Authorization is required.**
    /// 
    /// **Response Scenarios:**
    /// - **200 OK:** The user's account has been successfully deleted.
    /// - **401 Unauthorized:** The user is not authenticated or is trying to delete an account that does not belong to them.
    /// - **404 Not Found:** The provided BusinessEntityId does not exist.
    /// - **500 Internal Server Error:** An unexpected error occurred while deleting the account.
    /// </remarks>
    /// <param name="id">The unique BusinessEntityId of the account to delete.</param>
    /// <returns>
    /// A success or error message depending on the operation result.
    /// </returns>
    /// <response code="200">The user's account has been successfully deleted.</response>
    /// <response code="401">The user is unauthorized or attempting to delete another user's account.</response>
    /// <response code="404">No account found with the provided BusinessEntityId.</response>
    /// <response code="500">An unexpected error occurred during account deletion.</response>
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("BusinessEntityId");

            if (userIdClaim == null)
            {
                return Unauthorized(new { Error = "User can only delete owned account. Identification failed" });
            }

            var businessEntityId = int.Parse(userIdClaim.Value);

            var result = await _userService.DeleteUserAsync(id);

            if (result)
            {
                return Ok(new { Message = "User deleted successfully." });
            }
            return StatusCode(500, new { Error = "An unexpected error occurred." });
        }
        catch (InvalidOperationException ex)
        {
            // Özel olarak fırlatılan hata
            return NotFound(new { Error = ex.Message });
        }

    }


}