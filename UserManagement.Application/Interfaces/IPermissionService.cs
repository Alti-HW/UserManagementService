using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Dtos.Permission;

namespace UserManagement.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> CreatePermissionAsync(PermissionRequestDto permissionRequest);
        Task<List<PermissionResponseDto>> GetPermissionsAsync();
        //Task<bool> UpdatePermissionAsync(UpdatePermissionRequestDto updatePermissionRequest);
        Task<bool> DeletePermissionAsync(string permissionId);
        //Task<PermissionResponseDto> GetPermissionByIdAsync(string permissionId);
    }
}
