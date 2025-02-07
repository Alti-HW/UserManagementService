using FluentValidation;
using UserManagement.Application.Dtos;

namespace UserManagement.Application.Validator;

public class RoleRepresentationRequestValidator : AbstractValidator<RoleRepresentationDto>
{
    public RoleRepresentationRequestValidator()
    {
        RuleFor(r => r.Id).NotNull().NotEmpty();
        RuleFor(r => r.Name).NotNull().NotEmpty();
    }
}
