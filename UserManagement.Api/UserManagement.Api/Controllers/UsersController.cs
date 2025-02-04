using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Params;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("Users")]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilterParams filterParams)
    {
        if (filterParams.First < 0 || filterParams.Max <= 0 || filterParams.Max > 100)
        {
            return BadRequest("Invalid pagination values. 'First' must be 0 or greater, and 'Max' must be between 1 and 100.");
        }

        var users = await userService.GetUsers(filterParams);

        if (users == null || !users.Any())
        {
            return NoContent();
        }

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserDto filterParams)
    {
        if (filterParams == null)
        {
            return BadRequest("User data is required.");
        }

        var response = await userService.CreateUser(filterParams);

        if (response == null)
        {
            return StatusCode(500, "User creation failed.");
        }

        return CreatedAtAction(nameof(GetUser), new { userId = response.Id }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserDto filterParams)
    {
        if (filterParams == null || filterParams.Id == Guid.Empty)
        {
            return BadRequest("Invalid user data.");
        }

        var response = await userService.PutUser(filterParams);

        if (response == false)
        {
            return NotFound("User not found.");
        }

        return NoContent();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var user = await userService.GetUser(new Guid(userId));

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
