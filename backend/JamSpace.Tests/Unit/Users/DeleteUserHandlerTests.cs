using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Users.Commands.Delete;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.Users;

public class DeleteUserHandlerTests
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

        var handler = new DeleteUserHandler(userRepo.Object, uow.Object);

        var cmd = new DeleteUserCommand(userId);

        // Act
        var act = async () => await handler.Handle(cmd, Ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Should_Mark_User_As_Deleted_And_SaveChanges_When_Not_Deleted_Yet()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            IsDeleted = false,
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        var handler = new DeleteUserHandler(userRepo.Object, uow.Object);

        var cmd = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        user.IsDeleted.Should().BeTrue();
        uow.Verify(u => u.SaveChangesAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Should_Leave_User_As_Deleted_If_Already_Deleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            IsDeleted = true,
            UserName = "testuser",
            DisplayName = "testuser"
        };

        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        var handler = new DeleteUserHandler(userRepo.Object, uow.Object);

        var cmd = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(cmd, Ct);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        user.IsDeleted.Should().BeTrue(); 
        uow.Verify(u => u.SaveChangesAsync(Ct), Times.Once);
    }
}
