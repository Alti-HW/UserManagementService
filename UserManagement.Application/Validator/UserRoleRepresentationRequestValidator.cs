using FluentValidation;
using UserManagement.Application.Dtos;

namespace UserManagement.Application.Validator;

public class UserRoleRepresentationRequestValidator : AbstractValidator<UserRoleRepresentationDto>
{
    public UserRoleRepresentationRequestValidator()
    {
        RuleFor(r => r.UserId).NotNull().NotEmpty();
        RuleForEach(r => r.RoleRepresentation).NotNull().SetValidator(new RoleRepresentationRequestValidator());
        RuleFor(r => r).NotNull().NotEmpty();
    }
}
