using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.Dtos.Role;

public interface IRoleService
{
    Task<bool> CreateClientRoleAsync(RoleRequestDto roleRequest);
    Task<List<RoleResponse>> GetClientRolesAsync();
    Task<bool> UpdateClientRoleAsync(UpdateRoleRequestDto updateRoleRequest);
    Task<bool> DeleteClientRoleAsync(string roleName);
    Task<RoleResponse> GetClientRoleByIdAsync(string roleId);
}
