using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement.Application.Constants;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;

namespace UserManagement.Api.Controllers;

/// <summary>
/// Controller for managing role mappings.
/// </summary>
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class RollMappingController : ControllerBase
{
    private readonly IRoleMappingService roleMappingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollMappingController"/> class.
    /// </summary>
    /// <param name="roleMappingService">The role mapping service.</param>
    public RollMappingController(IRoleMappingService roleMappingService)
    {
        this.roleMappingService = roleMappingService;
    }

    /// <summary>
    /// Retrieves the available client roles for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of available client roles.</returns>
    /// <response code="200">Returns the list of available client roles.</response>
    /// <response code="400">If the user ID is invalid.</response>
    /// <response code="404">If no roles are found.</response>
    [HttpGet("user/{userId}/available")]
    [SwaggerOperation(Summary = "Get available client roles", Description = "Fetches the available client roles for a given user ID.")]
    [SwaggerResponse(200, "Successful response", typeof(ApiResponse<IEnumerable<RoleRepresentationDto>>))]
    [SwaggerResponse(400, "Invalid user ID.")]
    [SwaggerResponse(404, "No roles found.")]
    public async Task<IActionResult> GetAvailableClientRoles([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var clientRoles = await roleMappingService.GetClientRoles(new Guid(userId));

        if (clientRoles == null || clientRoles?.Count() == 0)
        {
            return NotFound(new ApiResponse<IEnumerable<RoleRepresentationDto>>
            {
                Success = true,
                Message = ResponseMessages.NoDataFound,
            });
        }

        return Ok(new ApiResponse<IEnumerable<RoleRepresentationDto>>
        {
            Success = true,
            Message = ResponseMessages.DataRetrieved,
            Data = clientRoles ?? [new RoleRepresentationDto()]
        });
    }

    /// <summary>
    /// Retrieves the roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of assigned roles.</returns>
    /// <response code="200">Returns the user's assigned roles.</response>
    /// <response code="400">If the user ID is invalid.</response>
    /// <response code="404">If no roles are assigned to the user.</response>
    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Get user assigned roles", Description = "Fetches the roles assigned to a given user ID.")]
    [SwaggerResponse(200, ResponseMessages.DataRetrieved, typeof(ApiResponse<RealmMappingsResponseDto>))]
    [SwaggerResponse(400, "Invalid user ID.")]
    [SwaggerResponse(404, ResponseMessages.NoDataFound)]
    public async Task<IActionResult> GetRolesAssignedToUser([FromRoute] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var userAssignedRoles = await roleMappingService.GetUserAssignedRoles(new Guid(userId));

        if (userAssignedRoles == null)
        {
            return NotFound(new ApiResponse<RealmMappingsResponseDto>
            {
                Success = true,
                Message = ResponseMessages.NoDataFound,
            });
        }

        return Ok(new ApiResponse<RealmMappingsResponseDto>
        {
            Success = true,
            Message = ResponseMessages.DataRetrieved,
            Data = userAssignedRoles ?? new RealmMappingsResponseDto()
        });
    }

    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    /// <param name="userRoleRepresentationDto">The user role data.</param>
    /// <returns>Response indicating success or failure.</returns>
    /// <response code="200">If the role mapping was created successfully.</response>
    /// <response code="400">If the input data is invalid.</response>
    /// <response code="422">If the user role mapping creation failed.</response>
    [HttpPost("user")]
    [SwaggerOperation(Summary = "Assigns user role mapping", Description = "Creates a new role mapping for a user.")]
    [SwaggerResponse(200, ResponseMessages.RoleAssignmentSuccess)]
    [SwaggerResponse(400, "Invalid user data.")]
    [SwaggerResponse(422, ResponseMessages.RoleAssignmentFailed)]
    public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleRepresentationDto userRoleRepresentationDto)
    {
        var response = await roleMappingService.AssignRole(userRoleRepresentationDto);

        if (response == false)
        {
            return UnprocessableEntity(new ApiResponse<UserRoleRepresentationDto>
            {
                Success = false,
                Message = ResponseMessages.RoleAssignmentFailed,
            });
        }

        return Ok(new ApiResponse<UserRoleRepresentationDto>
        {
            Success = true,
            Message = ResponseMessages.RoleAssignmentSuccess,
        });
    }

    /// <summary>
    /// Unassigns a role from a user.
    /// </summary>
    /// <param name="userRoleRepresentationDto">User role data to unassign.</param>
    /// <returns>Response indicating success or failure.</returns>
    /// <response code="201">User role unassigned successfully.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="422">If unassignment fails.</response>
    [HttpDelete("user")]
    [SwaggerOperation(Summary = "Unassign role from user", Description = "Removes a role from a user.")]
    [SwaggerResponse(200, ResponseMessages.RoleUnassignmentSuccess, typeof(ApiResponse<UserRoleRepresentationDto>))]
    [SwaggerResponse(400, "Invalid user data.")]
    [SwaggerResponse(422, ResponseMessages.RoleUnassignmentFailed)]
    public async Task<IActionResult> UnAssignRoleForUser([FromBody] UserRoleRepresentationDto userRoleRepresentationDto)
    {
        var response = await roleMappingService.UnAssignRole(userRoleRepresentationDto);

        if (response == false)
        {
            return UnprocessableEntity(new ApiResponse<UserRoleRepresentationDto>
            {
                Success = false,
                Message = ResponseMessages.RoleUnassignmentFailed,
            });
        }

        return Ok(new ApiResponse<UserRoleRepresentationDto>
        {
            Success = true,
            Message = ResponseMessages.RoleUnassignmentSuccess,
        });
    }
}
