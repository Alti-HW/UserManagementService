using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Constants;
using UserManagement.Application.Dtos;
using UserManagement.Application.Interfaces;

namespace UserManagement.Api.Controllers
{
    /// <summary>
    /// Controller to handle Single Sign-On (SSO) operations.
    /// </summary>
    [ApiController]
    [Route("api/sso")]
    public class SsoController : Controller
    {
        private readonly ISsoService _ssoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SsoController"/> class.
        /// </summary>
        /// <param name="ssoService">The SSO service to handle authentication.</param>
        public SsoController(ISsoService ssoService)
        {
            _ssoService = ssoService;
        }

        /// <summary>
        /// Initiates the SSO login process by redirecting to the authentication provider.
        /// </summary>
        /// <returns>Redirects to the SSO provider's login page.</returns>
        [HttpGet("login")]
        public async Task<IActionResult> SsoLogin()
        {
            var redirectUrl = await _ssoService.GetRedirectUrlAsync();

            if (!Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ResponseMessages.InvalidRedirectUrl,
                });
            }
            
            // return Ok(new ApiResponse<object>
            // {
            //     Success = true,
            //     Message = ResponseMessages.DataRetrieved,
            //     Data = new 
            //     {
            //         RedirectUrl=redirectUrl,
            //     }
            // });   

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Handles the SSO callback by exchanging the authorization code for an access token.
        /// </summary>
        /// <param name="session_state"></param>
        /// <param name="iss"></param>
        /// <param name="code">The authorization code received from the SSO provider.</param>
        /// <returns>Redirects to the client application with the access token.</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> SsoCallback([FromQuery] string? session_state, [FromQuery] string? iss, [FromQuery] string code)
        {
            var response = await _ssoService.GetTokenAsync(code);

            if(response is null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ResponseMessages.InvalidAuthCode,
                });
            }
            
            return Redirect($"http://localhost:3000?token={response.Access_Token}&refreshToken={response.Refresh_Token}");
        }
    }
}