

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly IMapper _mapper;
    public UserController(IRegisterService registerService, IMapper mapper)
    {
        _registerService = registerService;
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
            await _registerService.RegisterUserAsync(dto);
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
}