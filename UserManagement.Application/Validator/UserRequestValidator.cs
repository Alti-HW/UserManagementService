using FluentValidation;
using UserManagement.Application.Dtos;

namespace UserManagement.Application.Validator;

public class UserRequestValidator : AbstractValidator<UserDto>
{
    public UserRequestValidator()
    {
        RuleFor(r => r.FirstName).NotNull().NotEmpty();
        RuleFor(r => r.LastName).NotNull().NotEmpty();
        RuleFor(r => r.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(r => r).NotNull().NotEmpty();
    }
}
