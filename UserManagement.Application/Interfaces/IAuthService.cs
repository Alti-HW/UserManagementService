using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Dtos.KeyCloak;

namespace UserManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<KeycloakTokenResponseDto> LoginAsync(string username, string password);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
