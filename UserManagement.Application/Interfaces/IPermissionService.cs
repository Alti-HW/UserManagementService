using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Permission;

namespace UserManagement.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<ApiResponse1<bool>>  CreatePermissionAsync(PermissionRequestDto permissionRequest);
        Task<List<PermissionResponseDto>> GetPermissionsAsync();
        //Task<bool> UpdatePermissionAsync(UpdatePermissionRequestDto updatePermissionRequest);
        Task<bool> DeletePermissionAsync(string permissionId);
        //Task<PermissionResponseDto> GetPermissionByIdAsync(string permissionId);
    }
}
