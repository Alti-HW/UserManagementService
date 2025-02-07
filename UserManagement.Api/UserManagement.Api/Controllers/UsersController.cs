using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement.Application.Constants;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Params;
using UserManagement.Application.Validator;

namespace UserManagement.Api.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Retrieves a list of users based on filters.
    /// </summary>
    /// <param name="filterParams">Filtering parameters for users.</param>
    /// <returns>A list of users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="400">If the filter parameters are invalid.</response>
    /// <response code="404">If no users are found.</response>
    [HttpGet("Users")]
    [SwaggerOperation(Summary = "Get users", Description = "Fetches a list of users based on provided filters.")]
    [SwaggerResponse(200, "Successful response", typeof(ApiResponse<IEnumerable<UserDto>>))]
    [SwaggerResponse(400, "Invalid pagination values.")]
    [SwaggerResponse(404, "No users found.")]
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

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="inputUser">User data.</param>
    /// <returns>Created user details.</returns>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="500">If the creation fails.</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Create user", Description = "Creates a new user.")]
    [SwaggerResponse(201, "User created successfully", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(400, "User data is required.")]
    [SwaggerResponse(500, "Internal server error.")]
    public async Task<IActionResult> Post([FromBody] UserDto inputUser)
    {
        var response = await userService.CreateUser(inputUser);

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

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="inputUser">User data to be updated.</param>
    /// <returns>Updated user details.</returns>
    /// <response code="200">User successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPut]
    [RuleSetForClientSideMessages("All", "Id")]
    [SwaggerOperation(Summary = "Update user", Description = "Updates an existing user.")]
    [SwaggerResponse(200, "User updated successfully", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(400, "Invalid user data.")]
    [SwaggerResponse(404, "User not found.")]
    public async Task<IActionResult> Put([FromBody] UserDto inputUser)
    {
        var response = await userService.PutUser(inputUser);

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

    /// <summary>
    /// Retrieves a user by ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>User details.</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="400">If the user ID is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpGet("{userId}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Fetches user details by user ID.")]
    [SwaggerResponse(200, "User found", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(400, "Invalid user ID.")]
    [SwaggerResponse(404, "User not found.")]
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
    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Response indicating success or failure.</returns>
    /// <response code="200">User successfully deleted.</response>
    /// <response code="400">If the user ID is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpDelete("{userId}")]
    [SwaggerOperation(Summary = "Delete user by ID", Description = "Deletes a user by their ID.")]
    [SwaggerResponse(200, "User deleted successfully", typeof(ApiResponse<UserDto>))]
    [SwaggerResponse(400, "Invalid user ID.")]
    [SwaggerResponse(404, "User not found.")]
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
