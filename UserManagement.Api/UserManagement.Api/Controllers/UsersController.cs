using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Constants;
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
            return BadRequest(new ApiResponse<IEnumerable<UserDto>>
            {
                Success = false,
                Message = "Invalid pagination values. 'First' must be 0 or greater, and 'Max' must be between 1 and 100.",
            });
        }

        var users = await userService.GetUsers(filterParams);

        if (users == null || !users.Any())
        {
            return NotFound(new ApiResponse<IEnumerable<UserDto>>
            {
                Success = false,
                Message = ResponseMessages.NoDataFound,
            });
        }

        return Ok(new ApiResponse<IEnumerable<UserDto>>
        {
            Success = true,
            Message = ResponseMessages.DataRetrieved,
            Data = users
        });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserDto filterParams)
    {
        if (filterParams == null)
        {
            return BadRequest(new ApiResponse<UserDto>
            {
                Success = false,
                Message = "User data is required.",
            });
        }

        var response = await userService.CreateUser(filterParams);

        if (response == null)
        {
            return StatusCode(500, new ApiResponse<UserDto>
            {
                Success = false,
                Message = "User creation failed due to an internal server error.",
            });
        }

        return CreatedAtAction(
            nameof(GetUser),
            new { userId = response.Id },
            new ApiResponse<UserDto>
            {
                Success = true,
                Message = ResponseMessages.RecordCreated.Replace("{Record}", "User"),
                Data = response
            });
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserDto filterParams)
    {
        if (filterParams == null || filterParams.Id == Guid.Empty)
        {
            return BadRequest(new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Invalid user data."
            });
        }

        var response = await userService.PutUser(filterParams);

        if (response == false)
        {
            return NotFound(new ApiResponse<UserDto>
            {
                Success = false,
                Message = ResponseMessages.RecordNotFound.Replace("{Record}", "User"),
            });
        }


        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = ResponseMessages.RecordUpdated.Replace("{Record}", "User")
        });
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Invalid user data.",
            });
        }

        var user = await userService.GetUser(new Guid(userId));

        if (user == null)
        {
            return NotFound(new ApiResponse<UserDto>
            {
                Success = false,
                Message = ResponseMessages.RecordNotFound.Replace("{Record}", "User"),
            });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = ResponseMessages.DataRetrieved,
            Data = user,
        });
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Invalid user data.",
            });
        }

        var response = await userService.DeleteUser(new Guid(userId));

        if (response == false)
        {
            return NotFound(new ApiResponse<UserDto>
            {
                Success = false,
                Message = ResponseMessages.RecordNotFound.Replace("{Record}", "User")
            });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = ResponseMessages.RecordDeleted.Replace("{Record}", "User")
        });
    }
}
