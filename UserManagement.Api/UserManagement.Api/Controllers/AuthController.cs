using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;

namespace UserManagement.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication-related operations.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns an access token.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>
        /// Returns a success response with the token if authentication is successful, 
        /// otherwise returns an error message.
        /// </returns>
        /// <response code="200">Returns the access token on successful login.</response>
        /// <response code="404">Returns when authentication fails due to invalid credentials.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);

            return string.IsNullOrEmpty(token)
                ? NotFound(new ApiResponse1<string>(false, "Invalid credentials", token))
                : Ok(new ApiResponse1<string>(true, "Login Successful.", token));
        }
    }
}
