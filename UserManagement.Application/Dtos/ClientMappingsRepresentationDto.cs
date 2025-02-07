#nullable disable

namespace UserManagement.Application.Dtos;

public class ClientMappingsRepresentationDto
{
    public string Id { get; set; }
    public string Client { get; set; }
    public List<RoleRepresentationDto> Mappings { get; set; }
}
