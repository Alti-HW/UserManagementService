using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.KeyCloak;
using UserManagement.Application.Interfaces;

namespace UserManagement.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login user and return access & refresh tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokenResponse = await _authService.LoginAsync(request.Email, request.Password);

            if (tokenResponse == null)
            {
                return NotFound(new ApiResponse1<string>(false, "Invalid credentials", null));
            }

            return Ok(new ApiResponse1<KeycloakTokenResponseDto>(true, "Login Successful.", tokenResponse));
        }

        /// <summary>
        /// Logout user by invalidating refresh token.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var result = await _authService.LogoutAsync(request.RefreshToken);

            return result
                ? Ok(new ApiResponse1<string>(true, "Logout successful.", null))
                : BadRequest(new ApiResponse1<string>(false, "Logout failed.", null));
        }
    }
}
