using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.Dtos.Role;
using UserManagement.Application.Models;

public interface IRoleService
{
    Task<bool> CreateClientRoleAsync(RoleRequestDto roleRequest,bool isCompositeRole);
    Task<List<RoleResponse>> GetClientRolesAsync();
    //Task<bool> UpdateClientRoleAsync(UpdateRoleRequestDto updateRoleRequest);
    Task<bool> DeleteClientRoleAsync(string roleName);
    Task<RoleResponseDto> GetClientRoleByIdAsync(string roleId);
}
