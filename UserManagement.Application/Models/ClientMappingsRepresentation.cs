#nullable disable

namespace UserManagement.Application.Models;

public class ClientMappingsRepresentation
{
    public string Id { get; set; }
    public string Client { get; set; }
    public List<RoleRepresentation> Mappings { get; set; }
}
