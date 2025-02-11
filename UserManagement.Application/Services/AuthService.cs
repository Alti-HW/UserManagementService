using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UserManagement.Application.Configuration;
using UserManagement.Application.Dtos.KeyCloak;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KeyCloakConfiguration _keycloakConfig;

        public AuthService(IHttpClientFactory httpClientFactory, IOptions<KeyCloakConfiguration> keycloakOptions)
        {
            _httpClientFactory = httpClientFactory;
            _keycloakConfig = keycloakOptions.Value;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenUrl = $"{_keycloakConfig.ServerUrl}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/token";

            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _keycloakConfig.ClientId),
                new KeyValuePair<string, string>("client_secret", _keycloakConfig.ClientSecret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var response = await client.PostAsync(tokenUrl, tokenRequest);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<KeycloakTokenResponseDto>(responseBody, options);

            return tokenResponse?.Access_Token;
        }
    }
}
