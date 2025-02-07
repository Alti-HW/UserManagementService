#nullable disable

using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Services;

public class ClientService : IClientService
{
    private readonly ITokenService tokenService;
    private readonly IRestClientService restClientService;
    private readonly IMapper mapper;
    private readonly KeyCloakConfiguration keyCloakConfiguration;

    public ClientService(ITokenService tokenService,
        IRestClientService restClientService,
        IOptions<KeyCloakConfiguration> keyCloakConfiguration,
        IMapper mapper)
    {
        this.tokenService = tokenService;
        this.restClientService = restClientService;
        this.mapper = mapper;
        this.keyCloakConfiguration = keyCloakConfiguration.Value;

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }
    }

    public async Task<IEnumerable<ClientDto>> GetClients(ClientsFilterParams clientsFilterParams)
    {
        if (clientsFilterParams == null)
        {
            throw new ArgumentNullException(nameof(ClientsFilterParams));
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/clients";
        var queryParameters = clientsFilterParams.ToFilteredDictionary();

        var clients = await this.restClientService.GetAsync<Client>(endpoint, token, queryParameters);

        if (clients == null)
        {
            return new List<ClientDto>();
        }

        return mapper.Map<IEnumerable<ClientDto>>(clients);
    }
}
