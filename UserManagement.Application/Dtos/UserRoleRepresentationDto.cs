#nullable disable

namespace UserManagement.Application.Dtos;

public class UserRoleRepresentationDto
{
    public Guid UserId { get; set; }

    public IEnumerable<RoleRepresentationDto> RoleRepresentation { get; set; }
}