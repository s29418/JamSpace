using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Commands.Register;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Domain.Entities;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace JamSpace.Tests.Unit.Authentication;

public class RegisterUserHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_Validation_When_Email_Already_Taken()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60
        };
        var jwtOptions = Options.Create(jwtSettings);

        var existing = new User
        {
            Id = Guid.NewGuid(),
            Email = "taken@example.com",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        userRepo
            .Setup(r => r.GetByEmailAsync("taken@example.com", Ct))
            .ReturnsAsync(existing);

        var handler = new RegisterUserHandler(userRepo.Object, jwt.Object, hasher.Object, jwtOptions);

        var cmd = new RegisterUserCommand("taken@example.com", "user", "Password123!");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email is already taken.");
    }

    [Fact]
    public async Task Should_Register_User_And_Return_AuthResult()
    {
        // Arrange
        var userRepo = new Mock<IUserRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var hasher = new Mock<IPasswordHasher>();

        var jwtSettings = new JwtSettings
        {
            AccessMinutes = 60
        };
        var jwtOptions = Options.Create(jwtSettings);

        userRepo
            .Setup(r => r.GetByEmailAsync("new@example.com", Ct))
            .ReturnsAsync((User?)null);

        hasher
            .Setup(h => h.Hash("Password123!"))
            .Returns("hashed-password");

        jwt
            .Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<int>(), jwtSettings.AccessMinutes))
            .Returns("access-token");

        var handler = new RegisterUserHandler(userRepo.Object, jwt.Object, hasher.Object, jwtOptions);

        var cmd = new RegisterUserCommand("new@example.com", "user", "Password123!");

        User? addedUser = null;
        userRepo
            .Setup(r => r.AddAsync(It.IsAny<User>(), Ct))
            .Callback<User, CancellationToken>((u, _) => addedUser = u)
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<AuthResultDto>();
        result.Username.Should().Be("user");
        result.Email.Should().Be("new@example.com");
        result.Token.Should().Be("access-token");

        addedUser.Should().NotBeNull();
        addedUser!.Email.Should().Be("new@example.com");
        addedUser.UserName.Should().Be("user");
        addedUser.DisplayName.Should().Be("user");
        addedUser.PasswordHash.Should().Be("hashed-password");
        addedUser.IsDeleted.Should().BeFalse();
    }
}
