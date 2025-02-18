using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Role;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleController"/> class.
    /// </summary>
    /// <param name="roleService">Service to handle role operations.</param>
    /// <param name="mapper">AutoMapper instance for mapping DTOs.</param>
    public RoleController(IRoleService roleService, IMapper mapper)
    {
        _roleService = roleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new client role.
    /// </summary>
    /// <param name="roleRequest">The role request DTO.</param>
    /// <returns>Returns a success or failure response.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleRequestDto roleRequest)
    {
        var result = await _roleService.CreateClientRoleAsync(roleRequest, true);
        return Ok(new ApiResponse1<bool>(result, result ? "Role created successfully" : "Failed to create role", result));
    }

    /// <summary>
    /// Retrieves a list of all client roles.
    /// </summary>
    /// <returns>Returns a list of client roles.</returns>
    [HttpGet("list")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetClientRolesAsync();

        // Check if roles are null or empty
        if (roles == null || !roles.Any())
        {
            return Ok(new ApiResponse1<List<RoleResponseDto>>(false, "Failed to fetch roles", new List<RoleResponseDto>()));
        }

        // Filter composite roles
        var filteredRoles = roles.Where(x => x.Composite).ToList();

        // Map filtered roles to RoleResponseDto using AutoMapper
        var mappedRoles = _mapper.Map<List<RoleResponseDto>>(filteredRoles);

        // Return the response with mapped roles
        return Ok(new ApiResponse1<List<RoleResponseDto>>(true, "Roles fetched successfully", mappedRoles));
    }

    /// <summary>
    /// Retrieves a client role by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    /// <returns>Returns the role details.</returns>
    [HttpGet("get")]
    public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
    {
        var role = await _roleService.GetClientRoleByIdAsync(roleId);

        return Ok(new ApiResponse1<RoleResponseDto>(
            role != null,
            role != null ? "Role details retrieved successfully" : "Role not found",
            role));
    }

    /// <summary>
    /// Deletes a client role by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    /// <returns>Returns a success or failure response.</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteRoleById([FromQuery] string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            return BadRequest(new ApiResponse1<bool>(false, "Role ID is required.", false));
        }

        try
        {
            var result = await _roleService.DeleteClientRoleAsync(roleId);
            return result
                ? Ok(new ApiResponse1<bool>(true, "Role deleted successfully.", true))
                : NotFound(new ApiResponse1<bool>(false, "Role not found or could not be deleted.", false));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse1<bool>(false, ex.Message, false));
        }
    }
    [HttpPost("update-rolepermissions")]
    public async Task<IActionResult> UpdateCompositeRoles([FromBody] UpdateCompositeRolesDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.RoleId))
        {
            return BadRequest(new ApiResponse1<bool>(false, "Invalid request: RoleId is required", false));
        }

        var result = await _roleService.UpdateCompositeRolesAsync(request.RoleId, request.RolePermissions);

        return result
            ? Ok(new ApiResponse1<bool>(true, "Composite roles updated successfully", true))
            : StatusCode(500, new ApiResponse1<bool>(false, "Failed to update composite roles", false));
    }
}
public class UpdateCompositeRolesDto
{
    public string RoleId { get; set; }  // Role ID for which composites should be updated
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); // List of assigned composite roles
}


