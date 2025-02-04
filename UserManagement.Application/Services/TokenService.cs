#nullable disable

namespace UserManagement.Application.Services;

public class TokenService : ITokenService
{
    private readonly KeyCloakConfiguration keyCloakConfiguration;

    public TokenService(IOptions<KeyCloakConfiguration> keyCloakConfiguration)
    {
        this.keyCloakConfiguration = keyCloakConfiguration.Value;
    }

    public async Task<string> GetBearerTokenAsync()
    {
        var options = new RestClientOptions(keyCloakConfiguration.TokenUrl);
        var client = new RestClient(options);

        var request = new RestRequest();

        // Add the necessary parameters for OAuth2 password grant
        request.AddParameter("grant_type", "password");
        request.AddParameter("client_id", keyCloakConfiguration.ClientId);
        request.AddParameter("client_secret", keyCloakConfiguration.ClientSecret);
        request.AddParameter("username", keyCloakConfiguration.Username);
        request.AddParameter("password", keyCloakConfiguration.Password);

        var response = await client.PostAsync(request);

        if (response.IsSuccessful)
        {
            dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content);
            return jsonResponse.access_token;
        }
        else
        {
            throw new Exception("Error retrieving token: " + response.ErrorMessage);
        }
    }
}
