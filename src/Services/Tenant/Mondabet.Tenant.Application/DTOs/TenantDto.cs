namespace Mondabet.Tenant.Application.DTOs;

public record TenantDto(
    Guid Id,
    string Code,
    string CompanyName,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? Slogan,
    string? Address,
    string? NationalAddress,
    double Latitude,
    double Longitude,
    string? BankAccount,
    string? ZakatNumber,
    DateTime SubscriptionEndDate,
    Guid PackageId,
    bool IsActive,
    string AdminEmail,
    string AdminMobile);

public record TenantCreateDto(
    string Code,
    string CompanyName,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? Slogan,
    string? Address,
    string? NationalAddress,
    double Latitude,
    double Longitude,
    string? BankAccount,
    string? ZakatNumber,
    DateTime SubscriptionEndDate,
    Guid PackageId,
    string AdminEmail,
    string AdminMobile);

public record TenantUpdateDto(
    string CompanyName,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? Slogan,
    string? Address,
    string? NationalAddress,
    double Latitude,
    double Longitude,
    string? BankAccount,
    string? ZakatNumber);

public record SubscriptionUpdateDto(Guid PackageId, DateTime EndDate);

public record TenantStatsDto(Guid TenantId, int ActiveEmployees, int TotalCheckIns);

public record PackageDto(
    Guid Id,
    string Name,
    int MaxUsers,
    decimal PriceMonthly,
    string FeaturesJson);

public record PackageCreateDto(
    string Name,
    int MaxUsers,
    decimal PriceMonthly,
    string FeaturesJson = "[]");
