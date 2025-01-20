

using System.Threading.Tasks;
using AutoMapper;
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
    public IActionResult Get()
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


}