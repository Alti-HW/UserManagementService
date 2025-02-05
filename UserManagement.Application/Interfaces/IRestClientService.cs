#nullable disable

namespace UserManagement.Application.Interfaces;

public interface IRestClientService
{
    Task<IEnumerable<T>> GetAsync<T>(string endpoint, string token, Dictionary<string, string> queryParameters = null);

    Task<RestResponse> SendPostRequestAsync<T>(string endpoint, string token, T body) where T : class;

    Task<RestResponse> SendPutRequestAsync<T>(string url, string token, T body) where T : class;

    Task<T> SendGetRequestAsync<T>(string endpoint, string token);

    Task<RestResponse> SendDeleteRequestAsync(string endpoint, string token);
}
