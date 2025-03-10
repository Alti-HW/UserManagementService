#nullable disable

namespace UserManagement.Application.Interfaces;

public interface IRestClientService
{
    Task<IEnumerable<T>> GetAsync<T>(string endpoint, string token, Dictionary<string, string> queryParameters = null);

    Task<RestResponse> SendPostRequestAsync<T>(string endpoint, string token, T body) where T : class;

    Task<RestResponse> SendPutRequestAsync<T>(string url, string token, T body) where T : class;

    /// <summary>
    /// Sends an asynchronous POST request to the specified URL with form data and deserializes the response into the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response into.</typeparam>
    /// <param name="url">The target URL for the POST request.</param>
    /// <param name="formData">A dictionary containing form data as key-value pairs.</param>
    /// <returns>The deserialized response of type T, or default(T) if the request fails.</returns>
    Task<T> SendPostRequestAsync<T>(string url, Dictionary<string, string> formData);

    Task<T> SendGetRequestAsync<T>(string endpoint, string token);

    Task<RestResponse> SendDeleteRequestAsync(string endpoint, string token);

    Task<RestResponse> SendDeleteRequestAsync<T>(string endpoint, string token, T body = null) where T : class;
}
