using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Queries.Login;
using JamSpace.Domain.Entities;
using Microsoft.Extensions.Options;

namespace JamSpace.Tests.Unit.Authentication;

public class LoginUserHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Found()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        };
        var jwtOptions = Options.Create(jwtSettings);

        userRepo
            .Setup(r => r.GetByEmailAsync("user@example.com", Ct))
            .ReturnsAsync((User?)null);

        var handler = new LoginUserHandler(
            userRepo.Object,
            refreshRepo.Object,
            jwt.Object,
            jwtOptions,
            hasher.Object
        );

        var query = new LoginUserQuery("user@example.com", "password", "127.0.0.1", "UA");

        // Act
        var act = async () => await handler.Handle(query, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Password_Invalid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "testuser",
            DisplayName = "testuser",
            PasswordHash = "hash"
        };

        var userRepo = new Mock<IUserRepository>();
        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        };
        var jwtOptions = Options.Create(jwtSettings);

        userRepo
            .Setup(r => r.GetByEmailAsync(user.Email, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("wrong-password", user.PasswordHash))
            .Returns(false);

        var handler = new LoginUserHandler(
            userRepo.Object,
            refreshRepo.Object,
            jwt.Object,
            jwtOptions,
            hasher.Object
        );

        var query = new LoginUserQuery(user.Email, "wrong-password", "127.0.0.1", "UA");

        // Act
        var act = async () => await handler.Handle(query, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("Invalid email or password.");

        refreshRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Return_Access_And_Refresh_Tokens_When_Valid_Credentials()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "testuser",
            DisplayName = "testuser",
            PasswordHash = "hash",
            TokenVersion = 0
        };

        var userRepo = new Mock<IUserRepository>();
        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        };
        var jwtOptions = Options.Create(jwtSettings);

        userRepo
            .Setup(r => r.GetByEmailAsync(user.Email, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("correct-password", user.PasswordHash))
            .Returns(true);

        jwt
            .Setup(j => j.GenerateAccessToken(user, user.TokenVersion, jwtSettings.AccessMinutes))
            .Returns("access-token");

        RefreshToken? savedToken = null;

        refreshRepo
            .Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((t, _) => savedToken = t)
            .Returns(Task.CompletedTask);

        var handler = new LoginUserHandler(
            userRepo.Object,
            refreshRepo.Object,
            jwt.Object,
            jwtOptions,
            hasher.Object
        );

        var query = new LoginUserQuery(user.Email, "correct-password", "UA","127.0.0.1");

        // Act
        var result = await handler.Handle(query, Ct);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.UserName.Should().Be(user.UserName);
        result.Email.Should().Be(user.Email);
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();

        // Assert
        refreshRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), Ct), Times.Once);

        // Assert
        savedToken.Should().NotBeNull();
        savedToken!.UserId.Should().Be(user.Id);
        savedToken.Token.Should().Be(result.RefreshToken);
        savedToken.IpAddress.Should().Be("127.0.0.1");
        savedToken.UserAgent.Should().Be("UA");
        savedToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}