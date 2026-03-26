using Mondabet.Shared.Domain;
using Mondabet.Tenant.Domain.Events;

namespace Mondabet.Tenant.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Code { get; private set; } = default!;
    public string CompanyName { get; private set; } = default!;
    public string? LogoUrl { get; private set; }
    public string PrimaryColor { get; private set; } = "#1A73E8";
    public string SecondaryColor { get; private set; } = "#FFFFFF";
    public string? Slogan { get; private set; }
    public string? Address { get; private set; }
    public string? NationalAddress { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? BankAccount { get; private set; }
    public string? ZakatNumber { get; private set; }
    public DateTime SubscriptionEndDate { get; private set; }
    public Guid PackageId { get; private set; }
    public bool IsActive { get; private set; }
    public string AdminEmail { get; private set; } = default!;
    public string AdminMobile { get; private set; } = default!;

    protected Tenant() { }

    public static Tenant Create(
        string code,
        string companyName,
        string adminEmail,
        string adminMobile,
        Guid packageId,
        DateTime subscriptionEndDate,
        string primaryColor = "#1A73E8",
        string secondaryColor = "#FFFFFF")
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Code = code.ToLowerInvariant(),
            CompanyName = companyName,
            AdminEmail = adminEmail,
            AdminMobile = adminMobile,
            PackageId = packageId,
            SubscriptionEndDate = subscriptionEndDate,
            PrimaryColor = primaryColor,
            SecondaryColor = secondaryColor,
            IsActive = true
        };
        tenant.AddDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.Code, adminEmail));
        return tenant;
    }

    public void Update(
        string companyName,
        string? logoUrl,
        string primaryColor,
        string secondaryColor,
        string? slogan,
        string? address,
        string? nationalAddress,
        double latitude,
        double longitude,
        string? bankAccount,
        string? zakatNumber)
    {
        CompanyName = companyName;
        LogoUrl = logoUrl;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        Slogan = slogan;
        Address = address;
        NationalAddress = nationalAddress;
        Latitude = latitude;
        Longitude = longitude;
        BankAccount = bankAccount;
        ZakatNumber = zakatNumber;
    }

    public void UpdateSubscription(Guid packageId, DateTime endDate)
    {
        PackageId = packageId;
        SubscriptionEndDate = endDate;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
