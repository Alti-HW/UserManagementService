using UserManagement.Application.Dtos.KeyCloak;

namespace UserManagement.Application.Interfaces
{
    public interface ISsoService
    {
        Task<string> GetRedirectUrlAsync(string provider = "");

        Task<KeycloakTokenResponseDto> GetTokenAsync(string authorizationCode);
    }
}
