using UserManagement.Application.Dtos;

namespace UserManagement.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetClients(ClientsFilterParams clientsFilterParams);
}
