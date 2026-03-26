using FluentValidation;

namespace Mondabet.Tenant.Application.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Dto.Code).NotEmpty().MaximumLength(50)
            .Matches("^[a-z0-9-]+$").WithMessage("Code must be lowercase alphanumeric with hyphens.");
        RuleFor(x => x.Dto.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.AdminEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Dto.AdminMobile).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Dto.PackageId).NotEmpty();
        RuleFor(x => x.Dto.SubscriptionEndDate).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.Dto.PrimaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.Dto.SecondaryColor).Matches("^#[0-9A-Fa-f]{6}$");
    }
}
