using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Users.Commands.ChangePassword;
using JamSpace.Domain.Entities;
using FluentValidation;
using Moq;

namespace JamSpace.Tests.Unit.Users;

public class ChangePasswordHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userRepo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var policy = new Mock<IPasswordPolicy>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync((User?)null);

        var handler = new ChangePasswordHandler(userRepo.Object, hasher.Object, policy.Object, uow.Object);

        var cmd = new ChangePasswordCommand(userId, "old", "newPassword123!");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_CurrentPassword_Is_Invalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PasswordHash = "hashed-password",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var policy = new Mock<IPasswordPolicy>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("wrong-current", user.PasswordHash))
            .Returns(false);

        var handler = new ChangePasswordHandler(userRepo.Object, hasher.Object, policy.Object, uow.Object);

        var cmd = new ChangePasswordCommand(userId, "wrong-current", "NewPassword123!");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainSingle(e =>
            e.PropertyName == "CurrentPassword" &&
            e.ErrorMessage == "Current password is incorrect.");

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_NewPassword_Does_Not_Meet_Policy()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PasswordHash = "hashed-password",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var policy = new Mock<IPasswordPolicy>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("correct-current", user.PasswordHash))
            .Returns(true);

        var errors = new Dictionary<string, string>
        {
            { "NewPassword", "Password must contain a digit." }
        };

        policy
            .Setup(p => p.Validate("weak"))
            .Returns((false, errors));

        var handler = new ChangePasswordHandler(userRepo.Object, hasher.Object, policy.Object, uow.Object);

        var cmd = new ChangePasswordCommand(userId, "correct-current", "weak");

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainSingle(e =>
            e.PropertyName == "NewPassword" &&
            e.ErrorMessage == "Password must contain a digit.");

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Update_Password_And_SaveChanges_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PasswordHash = "old-hash",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var policy = new Mock<IPasswordPolicy>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        hasher
            .Setup(h => h.Verify("old-password", user.PasswordHash))
            .Returns(true);

        policy
            .Setup(p => p.Validate("NewPassword123!"))
            .Returns((true, new Dictionary<string, string>()));

        hasher
            .Setup(h => h.Hash("NewPassword123!"))
            .Returns("new-hash");

        var handler = new ChangePasswordHandler(userRepo.Object, hasher.Object, policy.Object, uow.Object);

        var cmd = new ChangePasswordCommand(userId, "old-password", "NewPassword123!");

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        user.PasswordHash.Should().Be("new-hash");
        uow.Verify(u => u.SaveChangesAsync(Ct), Times.Once);
    }
}
