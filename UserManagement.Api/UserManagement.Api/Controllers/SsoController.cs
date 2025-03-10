using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserManagement.Application.Configuration;
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
        private readonly EmsUi _emsUi;

        /// <summary>
        /// Initializes a new instance of the <see cref="SsoController"/> class.
        /// </summary>
        /// <param name="ssoService">The SSO service to handle authentication.</param>
        /// <param name="emsUi">The ems UI configurations</param>
        public SsoController(ISsoService ssoService, IOptions<EmsUi> emsUi)
        {
            _ssoService = ssoService;
            _emsUi = emsUi.Value;
        }

        /// <summary>
        /// Initiates the SSO (Single Sign-On) login process by redirecting the user to the authentication provider.
        /// </summary>
        /// <param name="provider">The name of the SSO provider. If not specified, a default provider may be used.</param>
        /// <returns>
        /// Returns an HTTP redirection response to the SSO provider's login page.
        /// If the redirect URL is invalid, returns a BadRequest response with an error message.
        /// </returns>
        [HttpGet("login")]
        public async Task<IActionResult> SsoLogin([FromQuery] string provider = "")
        {
            var redirectUrl = await _ssoService.GetRedirectUrlAsync(provider);

            if (!Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri? uriResult) || uriResult is null ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ResponseMessages.InvalidRedirectUrl,
                });
            }
            
            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Handles the SSO (Single Sign-On) callback by exchanging the authorization code for an access token.
        /// </summary>
        /// <param name="session_state">Optional session state parameter returned by the SSO provider.</param>
        /// <param name="iss">Optional issuer identifier indicating the SSO provider.</param>
        /// <param name="code">The authorization code received from the SSO provider, required to obtain an access token.</param>
        /// <returns>
        /// Redirects the user to the client application with the obtained access and refresh tokens.
        /// If the authorization code is invalid, returns a BadRequest response with an error message.
        /// </returns>
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
            
            return Redirect($"{_emsUi.BaseUrl}?token={response.Access_Token}&refreshToken={response.Refresh_Token}");
        }
    }
}