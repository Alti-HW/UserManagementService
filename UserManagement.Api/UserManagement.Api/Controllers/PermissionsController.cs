using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Permission;
using UserManagement.Application.Interfaces;

[ApiController]
[Route("api/permissions")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionController"/> class.
    /// </summary>
    /// <param name="permissionService">Service to handle permission-related operations.</param>
    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="permissionRequest">The permission request DTO containing the details of the permission.</param>
    /// <returns>Returns a success or failure response.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreatePermission([FromBody] PermissionRequestDto permissionRequest)
    {
        var result = await _permissionService.CreatePermissionAsync(permissionRequest);
        return Ok(new ApiResponse1<bool>(result, result ? "Permission created successfully" : "Failed to create permission", result));
    }

    /// <summary>
    /// Retrieves a list of all permissions.
    /// </summary>
    /// <returns>Returns a list of permissions.</returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _permissionService.GetPermissionsAsync();
        return Ok(new ApiResponse1<List<PermissionResponseDto>>(permissions != null, permissions != null ? "Permissions fetched successfully" : "Failed to fetch permissions", permissions));
    }


    /// <summary>
    /// Deletes a permission by its ID.
    /// </summary>
    /// <param name="permissionId">The ID of the permission to be deleted.</param>
    /// <returns>Returns a success or failure response.</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeletePermissionById([FromQuery] string permissionId)
    {
        // Validate input
        if (string.IsNullOrEmpty(permissionId))
            return BadRequest(new ApiResponse1<bool>(false, "Permission ID is required.", false));

        try
        {
            var result = await _permissionService.DeletePermissionAsync(permissionId);
            return result
                ? Ok(new ApiResponse1<bool>(true, "Permission deleted successfully.", true))
                : NotFound(new ApiResponse1<bool>(false, "Permission not found or could not be deleted.", false));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse1<bool>(false, ex.Message, false));
        }
    }
}
