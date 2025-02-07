namespace UserManagement.Application.Extensions;

public static class TokenServiceExtensions
{
    public static async Task<string> GetToken(this ITokenService tokenService)
    {
        var token = await tokenService.GetBearerTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve bearer token.");
        }

        return token;
    }
}
