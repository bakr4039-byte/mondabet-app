using Mondabet.Shared.Domain;

namespace Mondabet.Tenant.Domain.Entities;

public class Package : BaseEntity
{
    public string Name { get; private set; } = default!;
    public int MaxUsers { get; private set; }
    public decimal PriceMonthly { get; private set; }
    public string FeaturesJson { get; private set; } = "[]";

    protected Package() { }

    public static Package Create(string name, int maxUsers, decimal priceMonthly, string featuresJson = "[]")
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            MaxUsers = maxUsers,
            PriceMonthly = priceMonthly,
            FeaturesJson = featuresJson
        };

    public void Update(string name, int maxUsers, decimal priceMonthly, string featuresJson)
    {
        Name = name;
        MaxUsers = maxUsers;
        PriceMonthly = priceMonthly;
        FeaturesJson = featuresJson;
    }
}
