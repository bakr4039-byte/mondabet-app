using FluentValidation;

namespace Mondabet.Identity.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TenantCode).NotEmpty().MaximumLength(50);
    }
}
