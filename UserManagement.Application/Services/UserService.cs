#nullable disable

using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;
using System.Linq;

namespace UserManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService tokenService;
        private readonly IRestClientService restClientService;
        private readonly KeyCloakConfiguration keyCloakConfiguration;

        public UserService(ITokenService tokenService, IRestClientService restClientService, IOptions<KeyCloakConfiguration> keyCloakConfiguration)
        {
            this.tokenService = tokenService;
            this.restClientService = restClientService;
            this.keyCloakConfiguration = keyCloakConfiguration.Value;
        }

        public async Task<List<UserDto>> GetUsers(UserFilterParams userFilterParams)
        {
            if (userFilterParams == null)
            {
                throw new ArgumentNullException(nameof(userFilterParams));
            }

            var token = await tokenService.GetBearerTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Failed to retrieve bearer token.");
            }

            if (this.keyCloakConfiguration == null)
            {
                throw new InvalidOperationException("Keycloak configuration is null.");
            }

            if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
            {
                throw new InvalidOperationException("Invalid Keycloak configuration values.");
            }

            var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";
            var queryParameters = userFilterParams.ToFilteredDictionary<UserFilterParams>();

            var users = await this.restClientService.GetAsync<Users>(endpoint, token, queryParameters);

            if (users == null)
            {
                return new List<UserDto>();
            }

            List<UserDto> results = new();

            foreach (var userProfile in users)
            {
                if (userProfile?.Id == null)
                {
                    continue;
                }

                results.Add(new UserDto
                {
                    Id = new Guid(userProfile.Id),
                    Email = userProfile.Email ?? string.Empty,
                    EmailVerified = userProfile.EmailVerified,
                    Enabled = userProfile.Enabled,
                    FirstName = userProfile.FirstName ?? string.Empty,
                    LastName = userProfile.LastName ?? string.Empty,
                    Username = userProfile.Username ?? string.Empty,
                    Created = userProfile.CreatedTimestamp.ToDateTimeOrNow(),
                });
            }

            return results;
        }

        public async Task<UserDto> CreateUser(UserDto inputUser)
        {
            if (inputUser == null)
            {
                throw new ArgumentNullException(nameof(inputUser));
            }

            var token = await tokenService.GetBearerTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Failed to retrieve bearer token.");
            }

            if (this.keyCloakConfiguration == null)
            {
                throw new InvalidOperationException("Keycloak configuration is null.");
            }

            if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
            {
                throw new InvalidOperationException("Invalid Keycloak configuration values.");
            }

            var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";
            var userId = inputUser.Id is null || inputUser.Id == Guid.Empty ? Guid.NewGuid() : inputUser.Id;

            var response = await this.restClientService.SendPostRequestAsync(endpoint, token, new Users()
            {
                Id = userId.ToString(),
                Email = inputUser.Email ?? string.Empty,
                EmailVerified = inputUser.EmailVerified,
                Enabled = inputUser.Enabled,
                FirstName = inputUser.FirstName ?? string.Empty,
                LastName = inputUser.LastName ?? string.Empty,
                Username = inputUser.Username ?? string.Empty,
            });

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

            var token = await tokenService.GetBearerTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Failed to retrieve bearer token.");
            }

            if (this.keyCloakConfiguration == null)
            {
                throw new InvalidOperationException("Keycloak configuration is null.");
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

            var response = await this.restClientService.SendPutRequestAsync(endpoint, token, new Users()
            {
                Id = inputUser.Id.ToString(),
                Email = inputUser.Email ?? string.Empty,
                EmailVerified = inputUser.EmailVerified,
                Enabled = inputUser.Enabled,
                FirstName = inputUser.FirstName ?? string.Empty,
                LastName = inputUser.LastName ?? string.Empty,
                Username = inputUser.Username ?? string.Empty,
            });

            return response?.IsSuccessStatusCode ?? false;
        }

        public async Task<UserDto> GetUser(Guid? userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            var token = await tokenService.GetBearerTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Failed to retrieve bearer token.");
            }

            if (this.keyCloakConfiguration == null)
            {
                throw new InvalidOperationException("Keycloak configuration is null.");
            }

            if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
            {
                throw new InvalidOperationException("Invalid Keycloak configuration values.");
            }

            var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}?userProfileMetadata=true";

            var user = await this.restClientService.SendGetRequestAsync<Users>(endpoint, token);

            if (user is null || string.IsNullOrEmpty(user.Id))
            {
                return null;
            }

            return new UserDto
            {
                Id = new Guid(user.Id),
                Email = user.Email ?? string.Empty,
                EmailVerified = user.EmailVerified,
                Enabled = user.Enabled,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Username = user.Username ?? string.Empty,
                Created = user.CreatedTimestamp.ToDateTimeOrNow(),
            };
        }

    }
}
