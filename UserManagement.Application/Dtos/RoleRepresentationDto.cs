#nullable disable

namespace UserManagement.Application.Dtos;

public class RoleRepresentationDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool? Composite { get; set; }

    public bool? ClientRole { get; set; }
}