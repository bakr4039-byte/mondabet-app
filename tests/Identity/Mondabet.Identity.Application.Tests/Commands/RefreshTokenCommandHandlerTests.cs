using FluentAssertions;
using Moq;
using Mondabet.Identity.Application.Commands.Refresh;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;
using Xunit;

namespace Mondabet.Identity.Application.Tests.Commands;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IJwtTokenService> _jwtService = new();
    private readonly Mock<IKeycloakService> _keycloak = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RefreshTokenCommandHandler CreateHandler()
        => new(_refreshRepo.Object, _userRepo.Object, _jwtService.Object,
            _keycloak.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewTokenPair()
    {
        var userId = Guid.NewGuid();
        var user = User.Create(Guid.NewGuid(), "test@example.com", "+966501234567");
        var token = RefreshToken.Create(userId, "old-token", 30);

        _refreshRepo.Setup(r => r.GetByTokenAsync("old-token", default)).ReturnsAsync(token);
        _userRepo.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _keycloak.Setup(k => k.GetUserRolesAsync(It.IsAny<string>(), default))
            .ReturnsAsync(new List<string> { "Employee" });
        _jwtService.Setup(j => j.GenerateAccessToken(user, It.IsAny<IEnumerable<string>>()))
            .Returns("new-access-token");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh-token");
        _refreshRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), default)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = CreateHandler();
        var result = await handler.Handle(new RefreshTokenCommand("old-token"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_ExpiredToken_ReturnsUnauthorized()
    {
        _refreshRepo.Setup(r => r.GetByTokenAsync("expired-token", default))
            .ReturnsAsync((RefreshToken?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(new RefreshTokenCommand("expired-token"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_RevokedToken_ReturnsUnauthorized()
    {
        var userId = Guid.NewGuid();
        var token = RefreshToken.Create(userId, "revoked-token", 30);
        token.Revoke();

        _refreshRepo.Setup(r => r.GetByTokenAsync("revoked-token", default)).ReturnsAsync(token);

        var handler = CreateHandler();
        var result = await handler.Handle(new RefreshTokenCommand("revoked-token"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Unauthorized");
    }
}
