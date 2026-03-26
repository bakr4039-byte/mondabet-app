using FluentAssertions;
using Moq;
using Mondabet.Identity.Application.Commands.Login;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Xunit;

namespace Mondabet.Identity.Application.Tests.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IKeycloakService> _keycloak = new();
    private readonly Mock<IOtpService> _otpService = new();

    private LoginCommandHandler CreateHandler()
        => new(_userRepo.Object, _keycloak.Object, _otpService.Object);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsMfaRequired()
    {
        var user = User.Create(Guid.NewGuid(), "test@example.com", "+966501234567");

        _keycloak.Setup(k => k.ValidateCredentialsAsync("test@example.com", "Password1!", default))
            .ReturnsAsync(true);

        _userRepo.Setup(r => r.GetByEmailAsync("test@example.com", default))
            .ReturnsAsync(user);

        _otpService.Setup(o => o.StoreSessionAsync(It.IsAny<string>(), user.Id, default))
            .Returns(Task.CompletedTask);

        _otpService.Setup(o => o.GenerateAndSendOtpAsync(It.IsAny<string>(), default))
            .ReturnsAsync("+966501234567");

        var handler = CreateHandler();
        var result = await handler.Handle(
            new LoginCommand("test@example.com", "Password1!", "tenant1"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.MfaRequired.Should().BeTrue();
        result.Value.SessionToken.Should().NotBeNullOrEmpty();
        result.Value.AccessToken.Should().BeNull();
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsUnauthorized()
    {
        _keycloak.Setup(k => k.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(
            new LoginCommand("bad@example.com", "wrong", "tenant1"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsForbidden()
    {
        var user = User.Create(Guid.NewGuid(), "inactive@example.com", "+966501234567");
        user.Deactivate();

        _keycloak.Setup(k => k.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(true);

        _userRepo.Setup(r => r.GetByEmailAsync("inactive@example.com", default))
            .ReturnsAsync(user);

        _userRepo.Setup(r => r.GetByMobileAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(
            new LoginCommand("inactive@example.com", "Password1!", "tenant1"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Forbidden");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUnauthorized()
    {
        _keycloak.Setup(k => k.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(true);

        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);

        _userRepo.Setup(r => r.GetByMobileAsync(It.IsAny<string>(), default))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(
            new LoginCommand("ghost@example.com", "Password1!", "tenant1"), default);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Unauthorized");
    }
}
