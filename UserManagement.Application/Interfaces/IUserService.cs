using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.User;

namespace UserManagement.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers(UserFilterParams userFilterParams);

    Task<UserDto> CreateUser(UserDto inputUser);

    Task<bool> PutUser(UserDto inputUser);

    Task<UserDto> GetUser(Guid? userId);

    Task<bool> DeleteUser(Guid? userId);

    Task<ApiResponse1<object>> InviteUserAsync(InviteUserDto inviteUserDto);
    //Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);

    //Task<bool> ResetPasswordAsync(string userId, string newPassword);
}
