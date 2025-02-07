#nullable disable

namespace UserManagement.Application.Dtos;

public class ClientDto
{
    public Guid Id { get; set; }

    public string ClientId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }
}
