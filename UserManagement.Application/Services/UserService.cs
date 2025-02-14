#nullable disable

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.User;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly ITokenService tokenService;
    private readonly IRestClientService restClientService;
    private readonly IMapper mapper;
    private readonly KeyCloakConfiguration keyCloakConfiguration;
    private readonly IConfiguration configuration;


    public UserService(ITokenService tokenService,
        IRestClientService restClientService,
        IOptions<KeyCloakConfiguration> keyCloakConfiguration,
        IMapper mapper)
    {
        this.tokenService = tokenService;
        this.restClientService = restClientService;
        this.mapper = mapper;
        this.keyCloakConfiguration = keyCloakConfiguration.Value;

        if (this.keyCloakConfiguration == null)
        {
            throw new InvalidOperationException("Keycloak configuration is null.");
        }

        if (string.IsNullOrEmpty(this.keyCloakConfiguration.ServerUrl) || string.IsNullOrEmpty(this.keyCloakConfiguration.Realm))
        {
            throw new InvalidOperationException("Invalid Keycloak configuration values.");
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsers(UserFilterParams userFilterParams)
    {
        if (userFilterParams == null)
        {
            throw new ArgumentNullException(nameof(userFilterParams));
        }

        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";
        var queryParameters = userFilterParams.ToFilteredDictionary();

        var users = await this.restClientService.GetAsync<Users>(endpoint, token, queryParameters);

        if (users == null)
        {
            return new List<UserDto>();
        }

        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateUser(UserDto inputUser)
    {
        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users";

        inputUser.Id = inputUser.Id is null || inputUser.Id == Guid.Empty ? Guid.NewGuid() : inputUser.Id;

        var response = await this.restClientService.SendPostRequestAsync(endpoint, token, mapper.Map<Users>(inputUser));

        if (response is null || response?.IsSuccessStatusCode is false)
        {
            ThrowErrorMessage(response);

            throw new InvalidOperationException("Error occurred while creating new user");
        }

        return (await this.GetUsers(new UserFilterParams
        {
            Exact = true,
            Username = inputUser.Username ?? string.Empty,
        })).FirstOrDefault();
    }

    public async Task<bool> PutUser(UserDto inputUser)
    {
        if (inputUser == null)
        {
            throw new ArgumentNullException(nameof(inputUser));
        }

        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{inputUser.Id}";

        var response = await this.restClientService.SendPutRequestAsync(endpoint, token, mapper.Map<Users>(inputUser));

        if (response is null || response?.IsSuccessStatusCode is false)
        {
            ThrowErrorMessage(response);

            throw new InvalidOperationException("Error occurred while creating new user");
        }

        return response?.IsSuccessStatusCode ?? false;
    }

    public async Task<UserDto> GetUser(Guid? userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}?userProfileMetadata=true";

        var user = await this.restClientService.SendGetRequestAsync<Users>(endpoint, token);

        if (user is null)
        {
            return null;
        }

        return mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUser(Guid? userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        var token = await tokenService.GetToken();
        var endpoint = $"{this.keyCloakConfiguration.ServerUrl}/admin/realms/{this.keyCloakConfiguration.Realm}/users/{userId}";

        var response = await this.restClientService.SendDeleteRequestAsync(endpoint, token);

        return response?.IsSuccessStatusCode ?? false;
    }    

    private static void ThrowErrorMessage(RestResponse response)
    {
        if (!string.IsNullOrWhiteSpace(response?.Content))
        {
            var errorMessage = GetErrorMessage(response);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

    private static string GetErrorMessage(RestResponse response)
    {
        var doc = JsonDocument.Parse(response?.Content);

        return doc.RootElement.GetProperty("errorMessage").GetString();
    }

    public async Task<ApiResponse1<object>> InviteUserAsync(InviteUserDto inviteUserDto)
    {
        if (string.IsNullOrEmpty(inviteUserDto.Email))
            return new ApiResponse1<object>(false, "Error: Email is required.", null);

        try
        {
            var token = await tokenService.GetToken();
            if (string.IsNullOrEmpty(token))
                return new ApiResponse1<object>(false, "Error: Failed to retrieve authentication token.", null);

            // Step 1: Create User in Keycloak
            var endpoint = $"{keyCloakConfiguration.ServerUrl}/admin/realms/{keyCloakConfiguration.Realm}/users";

            var newUser = new
            {
                username = inviteUserDto.Email,
                email = inviteUserDto.Email,
                firstName = inviteUserDto.FirstName,
                lastName = inviteUserDto.LastName,
                enabled = true,
                emailVerified = false
            };

            var response = await restClientService.SendPostRequestAsync(endpoint, token, newUser);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse1<object>(false, $"Error: Failed to create user.", null);

            // Step 2: Get Created User ID
            var createdUser = await GetUsers(new UserFilterParams { Exact = true, Username = inviteUserDto.Email });
            var userId = createdUser.FirstOrDefault()?.Id;

            if (userId == null)
                return new ApiResponse1<object>(false, "Error: User created, but failed to retrieve user ID.", null);

            // Step 3: Generate Keycloak Reset Password Email
            var emailEndpoint = $"{keyCloakConfiguration.ServerUrl}/admin/realms/{keyCloakConfiguration.Realm}/users/{userId}/execute-actions-email";
            var actions = new[] { "UPDATE_PASSWORD" };

            var emailResponse = await restClientService.SendPutRequestAsync(emailEndpoint, token, actions);

            if (!emailResponse.IsSuccessStatusCode)
                return new ApiResponse1<object>(false, $"Error: User created, but failed to send password reset email.", null);


            var responseData = new { UserId = userId};
            return new ApiResponse1<object>(true, "User invited successfully.", responseData);
        }
        catch (Exception ex)
        {
            return new ApiResponse1<object>(false, $"Error: An unexpected error occurred. {ex.Message}", null);
        }
    }

    private string GenerateInviteToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("invite", "true")
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task StoreInviteToken(Guid userId, string inviteToken)
    {
        var token = await tokenService.GetToken();
        var endpoint = $"{keyCloakConfiguration.ServerUrl}/admin/realms/{keyCloakConfiguration.Realm}/users/{userId}";
        var updateData = new { attributes = new { inviteToken } };
        await restClientService.SendPutRequestAsync(endpoint, token, updateData);
    }

    //private async Task<bool> SendEmail(string email, string inviteToken)
    //{
    //    string link = $"https://your-app.com/set-password?token={inviteToken}";
    //    string subject = "You're Invited - Set Your Password";
    //    string body = $"Click <a href='{link}'>here</a> to set your password.";
    //    return await emailService.SendEmailAsync(email, subject, body);
    //}

    public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
            throw new ArgumentException("User ID and Password cannot be empty.");

        var token = await tokenService.GetToken();
        var endpoint = $"{keyCloakConfiguration.ServerUrl}/admin/realms/{keyCloakConfiguration.Realm}/users/{userId}/reset-password";

        var passwordPayload = new
        {
            type = "password",
            value = newPassword,
            temporary = false
        };

        var response = await restClientService.SendPutRequestAsync(endpoint, token, passwordPayload);
        return response.IsSuccessStatusCode;
    }

    private Guid? ValidateInviteToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> UpdateUserPassword(Guid userId, string newPassword)
    {
        var token = await tokenService.GetToken();
        var endpoint = $"{keyCloakConfiguration.ServerUrl}/admin/realms/{keyCloakConfiguration.Realm}/users/{userId}/reset-password";

        var passwordPayload = new
        {
            type = "password",
            value = newPassword,
            temporary = false
        };

        var response = await restClientService.SendPutRequestAsync(endpoint, token, passwordPayload);
        return response.IsSuccessStatusCode;
    }




}
