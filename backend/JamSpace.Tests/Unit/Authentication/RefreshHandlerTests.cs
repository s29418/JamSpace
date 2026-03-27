using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Commands.RefreshToken;
using JamSpace.Domain.Entities;
using Microsoft.Extensions.Options;

namespace JamSpace.Tests.Unit.Authentication;

public class RefreshHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_Forbidden_When_No_Refresh_Token_Provided()
    {
        // Arrange
        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var uow = new Mock<IUnitOfWork>();

        var jwtOptions = Options.Create(new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        });

        var handler = new RefreshHandler(refreshRepo.Object, userRepo.Object, jwt.Object, uow.Object, jwtOptions);
        var cmd = new RefreshCommand(null!);

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Refresh_Token_Not_Found()
    {
        // Arrange
        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var uow = new Mock<IUnitOfWork>();

        var jwtOptions = Options.Create(new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        });

        refreshRepo
            .Setup(r => r.GetByTokenAsync("token", Ct))
            .ReturnsAsync((RefreshToken?)null);

        var handler = new RefreshHandler(refreshRepo.Object, userRepo.Object, jwt.Object, uow.Object, jwtOptions);
        var cmd = new RefreshCommand("token");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Refresh_Token_Inactive()
    {
        // Arrange
        var existing = new RefreshToken
        {
            Token = "token",
            UserId = Guid.NewGuid(),
            RevokedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var uow = new Mock<IUnitOfWork>();

        var jwtOptions = Options.Create(new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        });

        refreshRepo
            .Setup(r => r.GetByTokenAsync("token", Ct))
            .ReturnsAsync(existing);

        var handler = new RefreshHandler(refreshRepo.Object, userRepo.Object, jwt.Object, uow.Object, jwtOptions);
        var cmd = new RefreshCommand("token");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("Refresh token inactive.");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var existing = new RefreshToken
        {
            Token = "token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var uow = new Mock<IUnitOfWork>();

        var jwtOptions = Options.Create(new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        });

        refreshRepo
            .Setup(r => r.GetByTokenAsync("token", Ct))
            .ReturnsAsync(existing);

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync((User?)null);

        var handler = new RefreshHandler(refreshRepo.Object, userRepo.Object, jwt.Object, uow.Object, jwtOptions);
        var cmd = new RefreshCommand("token");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("User not found.");

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Rotate_Token_And_Return_New_Auth_Result()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "user",
            DisplayName = "user",
            TokenVersion = 5
        };

        var existing = new RefreshToken
        {
            Token = "token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        var refreshRepo = new Mock<IRefreshTokenRepository>();
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var uow = new Mock<IUnitOfWork>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60,
            RefreshDays = 7
        };
        var jwtOptions = Options.Create(jwtSettings);

        refreshRepo
            .Setup(r => r.GetByTokenAsync("token", Ct))
            .ReturnsAsync(existing);

        userRepo
            .Setup(r => r.GetByIdAsync(user.Id, Ct))
            .ReturnsAsync(user);

        jwt
            .Setup(j => j.GenerateAccessToken(user, user.TokenVersion, jwtSettings.AccessMinutes))
            .Returns("new-access");

        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RefreshHandler(refreshRepo.Object, userRepo.Object, jwt.Object, uow.Object, jwtOptions);
        var cmd = new RefreshCommand("token");

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.UserName.Should().Be(user.UserName);
        result.Email.Should().Be(user.Email);
        result.AccessToken.Should().Be("new-access");
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();

        refreshRepo.Verify(r => r.RotateAsync(
                existing,
                result.RefreshToken,
                It.IsAny<DateTime>(),
                Ct),
            Times.Once);

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}