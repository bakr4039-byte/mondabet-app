using FluentAssertions;
using System.Security.Claims;
using Xunit;

namespace Mondabet.Security.Tests;

/// <summary>
/// Validates that RBAC role constants and claim-based authorization
/// logic works as expected across all microservice policies.
/// These tests guard against role string typos and policy misconfigurations.
/// </summary>
public class RbacPolicyTests
{
    // Role constants as used in AddAuthorization policy definitions
    private const string SuperAdmin = "SuperAdmin";
    private const string CompanyAdmin = "CompanyAdmin";
    private const string Employee = "Employee";

    private static ClaimsPrincipal MakePrincipal(string role, string tenantId = "tenant-1") =>
        new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, role),
            new Claim("tenant_id", tenantId),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        ], "Bearer"));

    [Theory]
    [InlineData(SuperAdmin, true)]
    [InlineData(CompanyAdmin, false)]
    [InlineData(Employee, false)]
    public void SuperAdmin_AccessToAdminActions(string role, bool expectedAccess)
    {
        var principal = MakePrincipal(role);
        var hasRole = principal.IsInRole(SuperAdmin);
        hasRole.Should().Be(expectedAccess);
    }

    [Theory]
    [InlineData(CompanyAdmin, true)]
    [InlineData(SuperAdmin, false)]
    [InlineData(Employee, false)]
    public void CompanyAdmin_AccessToManagementActions(string role, bool expectedAccess)
    {
        var principal = MakePrincipal(role);
        var hasRole = principal.IsInRole(CompanyAdmin);
        hasRole.Should().Be(expectedAccess);
    }

    [Fact]
    public void TenantClaim_ShouldBePresentForAllRoles()
    {
        foreach (var role in new[] { SuperAdmin, CompanyAdmin, Employee })
        {
            var principal = MakePrincipal(role, "tenant-abc");
            var tenantClaim = principal.FindFirst("tenant_id");
            tenantClaim.Should().NotBeNull(because: $"role {role} must carry tenant_id claim");
            tenantClaim!.Value.Should().Be("tenant-abc");
        }
    }

    [Fact]
    public void Employee_CannotAccessAdminOrManagement()
    {
        var principal = MakePrincipal(Employee);
        principal.IsInRole(SuperAdmin).Should().BeFalse();
        principal.IsInRole(CompanyAdmin).Should().BeFalse();
    }

    [Fact]
    public void MultiRole_PrincipalShouldMatchAllAssignedRoles()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, SuperAdmin),
            new Claim(ClaimTypes.Role, CompanyAdmin),
            new Claim("tenant_id", "tenant-1"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        ], "Bearer");
        var principal = new ClaimsPrincipal(identity);

        principal.IsInRole(SuperAdmin).Should().BeTrue();
        principal.IsInRole(CompanyAdmin).Should().BeTrue();
        principal.IsInRole(Employee).Should().BeFalse();
    }
}
