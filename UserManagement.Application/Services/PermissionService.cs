using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Permission;
using UserManagement.Application.Dtos.Role;

namespace UserManagement.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly KeyCloakConfiguration _keycloakOptions;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public PermissionService(IOptions<KeyCloakConfiguration> keycloakOptions, IRoleService roleService, IMapper mapper)
        {
            _keycloakOptions = keycloakOptions.Value;
            _roleService = roleService;
            _mapper = mapper;
        }


        public Task<bool> CreatePermissionAsync(PermissionRequestDto permissionRequest)
        {

            var request= _mapper.Map<RoleRequestDto>(permissionRequest);
            var result = _roleService.CreateClientRoleAsync(request,false);
            return result;
        }

        public Task<bool> DeletePermissionAsync(string permissionId)
        {
            return _roleService.DeleteClientRoleAsync(permissionId);
        }



        public async Task<List<PermissionResponseDto>> GetPermissionsAsync()
        {
            var roles = await _roleService.GetClientRolesAsync(); // Fetch the roles
            var filteredRoles = roles.Where(x => x.Composite == false).ToList(); // Filter non-composite roles

            // Map the filtered roles to PermissionResponseDto using AutoMapper
            return _mapper.Map<List<PermissionResponseDto>>(filteredRoles);
        }




    }
}
