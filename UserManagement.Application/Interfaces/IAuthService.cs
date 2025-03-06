using UserManagement.Application.Dtos.KeyCloak;

namespace UserManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<KeycloakTokenResponseDto> LoginAsync(string username, string password);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
