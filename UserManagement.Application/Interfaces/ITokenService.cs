namespace UserManagement.Application.Interfaces;

public interface ITokenService
{
    Task<string> GetBearerTokenAsync();
}
