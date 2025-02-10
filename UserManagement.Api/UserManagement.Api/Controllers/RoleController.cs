using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Role;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleRequestDto roleRequest)
    {
        var result = await _roleService.CreateClientRoleAsync(roleRequest);
        return Ok(new ApiResponse1<bool>(result, result ? "Role created successfully" : "Failed to create role", result));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetClientRolesAsync();
        return Ok(new ApiResponse1<List<RoleResponse>>(roles != null, roles != null ? "Roles fetched successfully" : "Failed to fetch roles", roles));
    }
    [HttpGet("get")]
    public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
    {
        var role = await _roleService.GetClientRoleByIdAsync(roleId);
        return Ok(new ApiResponse1<RoleResponse>(
            role != null,
            role != null ? "Role details retrieved successfully" : "Role not found",
            role));
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequestDto updateRoleRequest)
    {
        var result = await _roleService.UpdateClientRoleAsync(updateRoleRequest);
        return result
            ? Ok(new ApiResponse1<bool>(true, "Role updated successfully", true))
            : BadRequest(new ApiResponse1<bool>(false, "Failed to update role", false));
    }


    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteRoleById([FromQuery] string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
            return BadRequest(new ApiResponse1<bool>(false, "Role ID is required.", false));

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

}
