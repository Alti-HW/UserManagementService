using UserManagement.Application.Dtos.KeyCloak;

namespace UserManagement.Application.Services
{
    public class SsoService : ISsoService
    {
        private readonly KeyCloakConfiguration _keycloakConfig;
        private readonly IRestClientService _restClientService;

        public SsoService(IRestClientService restClientService, IOptions<KeyCloakConfiguration> keycloakOptions)
        {
            _keycloakConfig = keycloakOptions.Value;
            _restClientService = restClientService;
        }

        public Task<string> GetRedirectUrlAsync()
        {
            var url = $"{_keycloakConfig.ServerUrl}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/auth" +
                      $"?client_id={_keycloakConfig.ClientId}" +
                      $"&redirect_uri={_keycloakConfig.RedirectUri}" +
                      "&response_type=code" +
                      "&scope=openid";

            return Task.FromResult(url);
        }

        public async Task<KeycloakTokenResponseDto> GetTokenAsync(string authorizationCode)
        {
            var tokenUrl = $"{_keycloakConfig.ServerUrl}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/token";

            var formData = new Dictionary<string, string>
            {
                { "client_id", _keycloakConfig.ClientId },
                { "client_secret", _keycloakConfig.ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "redirect_uri", _keycloakConfig.RedirectUri }
            };
            
            var keycloakTokenResponse = await _restClientService.SendPostRequestAsync<KeycloakTokenResponseDto>(tokenUrl, formData);

            return keycloakTokenResponse;
        }
    }
}
