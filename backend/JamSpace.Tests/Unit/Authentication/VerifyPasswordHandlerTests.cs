using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Authentication.Queries.VerifyPassword;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Authentication;

public class VerifyPasswordHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Found()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();

        var userId = Guid.NewGuid();

        repo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync((User?)null);

        var handler = new VerifyPasswordHandler(repo.Object, hasher.Object);

        var query = new VerifyPasswordQuery(userId, "password");

        // Act
        var act = async () => await handler.Handle(query, Ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Password_Invalid()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PasswordHash = "hash",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        repo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("wrong", user.PasswordHash))
            .Returns(false);

        var handler = new VerifyPasswordHandler(repo.Object, hasher.Object);

        var query = new VerifyPasswordQuery(userId, "wrong");

        // Act
        var act = async () => await handler.Handle(query, Ct);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("Invalid password.");
    }

    [Fact]
    public async Task Should_Return_Unit_When_Password_Is_Correct()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PasswordHash = "hash",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        repo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("correct", user.PasswordHash))
            .Returns(true);

        var handler = new VerifyPasswordHandler(repo.Object, hasher.Object);

        var query = new VerifyPasswordQuery(userId, "correct");

        // Act
        var result = await handler.Handle(query, Ct);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
    }
}
