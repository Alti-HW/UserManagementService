using UserManagement.Application.Dtos;

namespace UserManagement.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers(UserFilterParams userFilterParams);

    Task<UserDto> CreateUser(UserDto inputUser);

    Task<bool> PutUser(UserDto inputUser);

    Task<UserDto> GetUser(Guid? userId);

    Task<bool> DeleteUser(Guid? userId);
}
