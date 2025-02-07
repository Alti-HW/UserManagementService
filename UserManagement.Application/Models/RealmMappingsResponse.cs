#nullable disable

namespace UserManagement.Application.Models;

public class RealmMappingsResponse
{
    public List<RoleRepresentation> RealmMappings { get; set; }
    public Dictionary<string, ClientMappingsRepresentation> ClientMappings { get; set; }
}
