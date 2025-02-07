#nullable disable

namespace UserManagement.Application.Dtos;

public class RealmMappingsResponseDto
{
    public List<RoleRepresentationDto> RealmMappings { get; set; }

    public Dictionary<string, ClientMappingsRepresentationDto> ClientMappings { get; set; }
}
