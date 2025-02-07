using UserManagement.Application.Dtos;

namespace UserManagement.Application.Interfaces;

public interface IRoleMappingService
{
    Task<IEnumerable<RoleRepresentationDto>> GetClientRoles(Guid userId);

    Task<RealmMappingsResponseDto> GetUserAssignedRoles(Guid userId);

    Task<bool> AssignRole(UserRoleRepresentationDto inputUserRoleRepresentationDto);

    Task<bool> UnAssignRole(UserRoleRepresentationDto inputUserRoleRepresentationDto);
}
