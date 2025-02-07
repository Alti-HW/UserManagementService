using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Services;

public class RoleMappingService : IRoleMappingService
{
    private readonly ITokenService tokenService;
    private readonly IRestClientService restClientService;
    private readonly IClientService clientService;
    private readonly KeyCloakConfiguration keyCloakConfiguration;
    private readonly IMapper mapper;

    public RoleMappingService(ITokenService tokenService,
        IRestClientService restClientService,
        IOptions<KeyCloakConfiguration> keyCloakConfiguration,
        IClientService clientService,
        IMapper mapper)
    {
        this.tokenService = tokenService;
        this.mapper = mapper;
        this.restClientService = restClientService;
        this.clientService = clientService;
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

        this.tokenService = tokenService;
    }

    public async Task<IEnumerable<RoleRepresentationDto>> GetClientRoles(Guid userId)
    {
        var clientId = await GetClientId();
        var token = await tokenService.GetToken();

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}" +
                       $"/users/{userId}/role-mappings/clients/{clientId}/available";

        var roleRepresentations = await this.restClientService.GetAsync<RoleRepresentation>(endpoint, token);

        if (roleRepresentations == null)
        {
            return new List<RoleRepresentationDto>();
        }

        return mapper.Map<IEnumerable<RoleRepresentationDto>>(roleRepresentations);
    }

    public async Task<RealmMappingsResponseDto> GetUserAssignedRoles(Guid userId)
    {
        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}/role-mappings";

        var roleRepresentations = await this.restClientService.SendGetRequestAsync<RealmMappingsResponseDto>(endpoint, token);

        if (roleRepresentations == null)
        {
            return new RealmMappingsResponseDto();
        }

        return mapper.Map<RealmMappingsResponseDto>(roleRepresentations);
    }

    public async Task<bool> AssignRole(UserRoleRepresentationDto inputUserRoleRepresentationDto)
    {
        var roleRepresentation = mapper.Map<List<RoleRepresentation>>(inputUserRoleRepresentationDto.RoleRepresentation);

        var clientId = await GetClientId();
        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{inputUserRoleRepresentationDto.UserId}" +
                       $"/role-mappings/clients/{clientId}";

        var response = await this.restClientService.SendPostRequestAsync(endpoint, token, roleRepresentation);

        if (response?.IsSuccessStatusCode is false)
        {
            throw new InvalidOperationException(response?.ErrorMessage, response?.ErrorException?.InnerException);
        }

        return response?.IsSuccessStatusCode ?? false;
    }

    public async Task<bool> UnAssignRole(UserRoleRepresentationDto inputUserRoleRepresentationDto)
    {
        var roleRepresentation = mapper.Map<List<RoleRepresentation>>(inputUserRoleRepresentationDto.RoleRepresentation);
        var clientId = await GetClientId();

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{inputUserRoleRepresentationDto.UserId}" +
                       $"/role-mappings/clients/{clientId}";

        var response = await this.restClientService.SendDeleteRequestAsync(endpoint, token,roleRepresentation );

        if (response?.IsSuccessStatusCode is false)
        {
            throw new InvalidOperationException(response?.ErrorMessage, response?.ErrorException?.InnerException);
        }

        return response?.IsSuccessStatusCode ?? false;
    }

    private async Task<Guid?> GetClientId()
    {
        var client = (await clientService.GetClients(new ClientsFilterParams()
        {
            ClientId = this.keyCloakConfiguration.ClientId,
        })).FirstOrDefault();

        if (client is null || client?.Id is null || client?.Id == Guid.Empty)
        {
            throw new InvalidOperationException("Invalid client id configuration.");
        }

        return client?.Id;
    }
}
