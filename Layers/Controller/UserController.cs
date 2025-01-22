

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
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put([FromBody] RegisterVM model)
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
    /// Retrieves a list of constants for user registration.
    /// </summary>
    /// <remarks>
    /// **Responses:**
    /// - `200 OK` : Both `States` and `AddressTypes` are fully populated.
    /// - `206 Partial Content` : One of the lists (`States` or `AddressTypes`) is empty, or both.
    /// - `204 No Content` : Both `States` and `AddressTypes` are empty.
    /// </remarks>
    /// <returns>A list of constants for user registration.</returns>
    [HttpGet("Constants")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressConstants))]
    [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(AddressConstants))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetConstants()
    {
        var answer = new AddressConstants();
        var statesDto = _userService.GetAllStates();
        if (statesDto.Count > 0)
        {
            answer.States = _mapper.Map<List<StateVM>>(statesDto);
        }
        var typesDto = _userService.GetAddressTypes();
        if (typesDto.Count > 0)
        {
            answer.AddressTypes = _mapper.Map<List<AddressTypeVM>>(typesDto);
        }

        if (answer.States.Count > 0 && answer.AddressTypes.Count > 0)
        {
            return Ok(answer);
        }
        else if (answer.States.Count > 0 || answer.AddressTypes.Count > 0)
        {
            return StatusCode(StatusCodes.Status206PartialContent, answer);
        }
        else
        {
            return NoContent();
        }

    }

    /// <summary>
    /// Retrieves the user information for the **authenticated** user.
    /// </summary>
    /// <remarks>
    /// **!!Does not returns Password info**
    /// **Responses:**
    /// - `200 OK` : Returns the user information.
    /// - `404 Not Found` : If the user is not found.
    /// - `500 Internal Server Error` : If an unexpected error occurs.
    /// </remarks>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserVM))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var businessEntityId = Token.GetBusinessEntityId(User);
        var infoDTO = new UpdateUserDTO();
        try
        {
            infoDTO = await _userService.GetUserInfo(businessEntityId);
        }
        catch (InvalidOperationException ex)
        {

            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
        return Ok(_mapper.Map<UpdateUserVM>(infoDTO));
    }

    /// <summary>
    /// Validates a user login attempt.
    /// </summary>
    /// <param name="Login">The login data (email and password).</param>
    /// <returns>Returns detailed messages for each validation step.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var result = await _userService.ValidateUserAsync(login);

        if (!result.IsSuccessful)
            return BadRequest(new { result.Message });

        return Ok(new { result.Message, result.Token });
    }

    /// <summary>
    /// Deletes a user's account based on the provided BusinessEntityId.
    /// </summary>
    /// <remarks>
    /// This endpoint allows an authenticated user to delete their **own** account. 
    /// 
    /// **Authorization is required.**
    /// 
    /// **Response Scenarios:**
    /// - **200 OK:** The user's account has been successfully deleted.
    /// - **401 Unauthorized:** The user is not authenticated or is trying to delete an account that does not belong to them.
    /// - **404 Not Found:** The provided BusinessEntityId does not exist.
    /// - **500 Internal Server Error:** An unexpected error occurred while deleting the account.
    /// </remarks>
    /// <returns>
    /// A success or error message depending on the operation result.
    /// </returns>
    /// <response code="200">The user's account has been successfully deleted.</response>
    /// <response code="401">The user is unauthorized or attempting to delete another user's account.</response>
    /// <response code="404">No account found with the provided BusinessEntityId.</response>
    /// <response code="500">An unexpected error occurred during account deletion.</response>
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var businessEntityId = Token.GetBusinessEntityId(User);

            var result = await _userService.DeleteUserAsync(businessEntityId);

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

    /// <summary>
    /// Updates the details of the authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint allows an authenticated user to update their account details. 
    /// The user can only update their own account, determined by the `BusinessEntityId` extracted from the token.
    /// - If the `BusinessEntityId` in the request does not match the authenticated user's ID, the operation will be unauthorized.
    /// - Null fields in the request body will be ignored and not updated.
    ///
    /// Possible Responses:
    /// - 200 OK: Returns a success message if the update is successful.
    /// - 400 Bad Request: If the update operation fails due to invalid input.
    /// - 401 Unauthorized: If the user is not authenticated or attempts to update another account.
    /// - 404 Not Found: If the user does not exist.
    /// - 500 Internal Server Error: For unexpected server errors.
    ///
    /// </remarks>
    /// <param name="userVM">The update details for the user, containing fields to be updated.</param>
    /// <returns>A response indicating the success or failure of the operation.</returns>
    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> Update(UpdateUserVM userVM)
    {
        try
        {
            var businessEntityId = Token.GetBusinessEntityId(User);

            var dto = _mapper.Map<UpdateUserDTO>(userVM);
            var isUpdated = await _userService.UpdateUserAsync(dto, businessEntityId);

            if (isUpdated)
            {
                return Ok(new { Message = "User updated successfully." });
            }
            return BadRequest(new { Error = "User update failed. Please check the input data." });
        }
        catch (InvalidOperationException ex)
        {
            // kullanici kaynakli hatalar
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            // beklenmeyen hatalar
            return StatusCode(500, new { Error = "An unexpected error occurred while updating the user.", Details = ex.Message });
        }
    }
}