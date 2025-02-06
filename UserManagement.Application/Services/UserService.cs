#nullable disable

using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly ITokenService tokenService;
    private readonly IRestClientService restClientService;
    private readonly IMapper mapper;
    private readonly KeyCloakConfiguration keyCloakConfiguration;

    public UserService(ITokenService tokenService,
        IRestClientService restClientService,
        IOptions<KeyCloakConfiguration> keyCloakConfiguration,
        IMapper mapper)
    {
        this.tokenService = tokenService;
        this.restClientService = restClientService;
        this.mapper = mapper;
        this.keyCloakConfiguration = keyCloakConfiguration.Value;
    }

    public async Task<IEnumerable<UserDto>> GetUsers(UserFilterParams userFilterParams)
    {
        if (userFilterParams == null)
        {
            throw new ArgumentNullException(nameof(userFilterParams));
        }

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";
        var queryParameters = userFilterParams.ToFilteredDictionary();

        var users = await this.restClientService.GetAsync<Users>(endpoint, token, queryParameters);

        if (users == null)
        {
            return new List<UserDto>();
        }

        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateUser(UserDto inputUser)
    {
        if (inputUser == null)
        {
            throw new ArgumentNullException(nameof(UserDto));
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";

        inputUser.Id = inputUser.Id is null || inputUser.Id == Guid.Empty ? Guid.NewGuid() : inputUser.Id;

        var response = await this.restClientService.SendPostRequestAsync(endpoint, token, mapper.Map<Users>(inputUser));

        if (response is null || response?.IsSuccessStatusCode is false)
        {
            return null;
        }

        return (await this.GetUsers(new UserFilterParams
        {
            Exact = true,
            Username = inputUser.Username ?? string.Empty,
        })).FirstOrDefault();
    }

    public async Task<bool> PutUser(UserDto inputUser)
    {
        if (inputUser == null)
        {
            throw new ArgumentNullException(nameof(inputUser));
        }

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }

        if (inputUser.Id == null)
        {
            throw new ArgumentException("User ID cannot be null.", nameof(inputUser.Id));
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{inputUser.Id}";

        var response = await this.restClientService.SendPutRequestAsync(endpoint, token, mapper.Map<Users>(inputUser));

        return response?.IsSuccessStatusCode ?? false;
    }

    public async Task<UserDto> GetUser(Guid? userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}?userProfileMetadata=true";

        var user = await this.restClientService.SendGetRequestAsync<Users>(endpoint, token);

        if (user is null)
        {
            return null;
        }

        return mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUser(Guid? userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }

        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}";

        var response = await this.restClientService.SendDeleteRequestAsync(endpoint, token);

        return response?.IsSuccessStatusCode ?? false;
    }
}
