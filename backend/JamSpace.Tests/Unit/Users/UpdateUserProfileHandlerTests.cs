using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Users.Commands.UpdateUserProfile;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Users;

public class UpdateUserProfileHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync((User?)null);

        var handler = new UpdateUserProfileHandler(userRepo.Object, uow.Object);

        var cmd = new UpdateUserProfileCommand(
            userId,
            false, null,
            false, null,
            false, null,
            false, null,
            false, null
        );

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Should_Update_DisplayName_Bio_And_ProfilePicture_When_Flags_Are_Set()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            DisplayName = "Old Name",
            Bio = "Old bio",
            ProfilePictureUrl = "old.png",
            UserName = "olduser"
        };

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        var handler = new UpdateUserProfileHandler(userRepo.Object, uow.Object);

        var cmd = new UpdateUserProfileCommand(
            userId,
            true, "  New Name  ",
            true, "  New bio  ",
            true, "new.png",
            false, null, 
            false, null 
        );

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        user.DisplayName.Should().Be("New Name");
        user.Bio.Should().Be("New bio");
        user.ProfilePictureUrl.Should().Be("new.png");

        result.Should().NotBeNull();
        uow.Verify(u => u.SaveChangesAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Conflict_When_Email_Already_In_Use_By_Other_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "old@example.com",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var existingUser = new User
        {
            Id = existingUserId,
            Email = "new@example.com",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        userRepo
            .Setup(r => r.GetByEmailAsync("new@example.com", Ct))
            .ReturnsAsync(existingUser);

        var handler = new UpdateUserProfileHandler(userRepo.Object, uow.Object);

        var cmd = new UpdateUserProfileCommand(
            userId,
            false, null,
            false, null,
            false, null,    
            false, null,
            true, "  new@example.com  "
        );

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Email already in use.");

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Update_Email_When_Not_Used_By_Other_User()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "old@example.com",
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        userRepo
            .Setup(r => r.GetByEmailAsync("new@example.com", Ct))
            .ReturnsAsync((User?)null);

        var handler = new UpdateUserProfileHandler(userRepo.Object, uow.Object);

        var cmd = new UpdateUserProfileCommand(
            userId,
            false, null,
            false, null,
            false, null,
            false, null,
            true, "  new@example.com  "
        );

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        user.Email.Should().Be("new@example.com");
        result.Should().NotBeNull(); 
        uow.Verify(u => u.SaveChangesAsync(Ct), Times.Once);
    }
}
