#nullable disable

using System.Text.Json;

namespace UserManagement.Application.Services;

public class RestClientService : IRestClientService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<T>> GetAsync<T>(string endpoint, string token, Dictionary<string, string> queryParameters = null)
    {
        var options = new RestClientOptions(endpoint);
        var client = new RestClient(options);

        var request = new RestRequest();

        // Add Authorization Header
        request.AddHeader("Authorization", $"Bearer {token}");

        if (queryParameters is not null)
        {
            foreach (var queryParameter in queryParameters)
            {
                request.AddQueryParameter(queryParameter.Key, queryParameter.Value);
            }
        }

        var response = await client.GetAsync(request);

        if (response is null || !response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
        {
            return new List<T>();
        }

        return JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content);
    }

    public async Task<RestResponse> SendPostRequestAsync<T>(string url, string token, T body) where T : class
    {
        var options = new RestClientOptions(url);
        var client = new RestClient(options);

        var request = new RestRequest()
            .AddHeader("Authorization", $"Bearer {token}")
            .AddHeader("Content-Type", "application/json")
            .AddJsonBody(body);

        return await client.ExecutePostAsync(request);
    }

    public async Task<RestResponse> SendPutRequestAsync<T>(string url, string token, T body) where T : class
    {
        var client = new RestClient();
        var request = new RestRequest(url, Method.Put);

        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(body);

        return await client.ExecuteAsync(request);
    }

    public async Task<T> SendGetRequestAsync<T>(string endpoint, string token)
    {
        var options = new RestClientOptions(endpoint);
        var client = new RestClient(options);

        var request = new RestRequest();
        request.AddHeader("Authorization", $"Bearer {token}");

        var response = await client.ExecuteAsync(request);

        if (response is null || !response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
        {
            return default;
        }

        return System.Text.Json.JsonSerializer.Deserialize<T>(response.Content, _jsonSerializerOptions);
    }

    public async Task<RestResponse> SendDeleteRequestAsync(string endpoint, string token)
    {
        var client = new RestClient();

        var request = new RestRequest(endpoint, Method.Delete);
        request.AddHeader("Authorization", $"Bearer {token}");

        return await client.ExecuteAsync(request);
    }

    public async Task<RestResponse> SendDeleteRequestAsync<T>(string endpoint, string token, T body) where T : class
    {
        var client = new RestClient();

        var request = new RestRequest(endpoint, Method.Delete);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("Content-Type", "application/json");

        request.AddJsonBody(body);

        return await client.ExecuteAsync(request);
    }
}
