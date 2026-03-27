using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.Users.Queries.GetDetails;
using JamSpace.Domain.Entities;
using Moq;

namespace JamSpace.Tests.Unit.Users;

public class GetUserByIdHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();

        var userRepo = new Mock<IUserRepository>();
        var followRepo = new Mock<IUserFollowRepository>();

        followRepo
            .Setup(r => r.UserFollowsAsync(requestingUserId, userId, Ct))
            .ReturnsAsync(false);

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByIdHandler(userRepo.Object, followRepo.Object);

        // Act
        var act = async () => await handler.Handle(
            new GetUserByIdQuery(userId, requestingUserId), Ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Should_Return_UserDto_When_User_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestingUserId = userId; 

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            DisplayName = "Test User",
            Email = "test@example.com"
        };

        var userRepo = new Mock<IUserRepository>();
        var followRepo = new Mock<IUserFollowRepository>();

        followRepo
            .Setup(r => r.UserFollowsAsync(requestingUserId, userId, Ct))
            .ReturnsAsync(true);

        userRepo
            .Setup(r => r.GetByIdAsync(userId, Ct))
            .ReturnsAsync(user);

        var handler = new GetUserByIdHandler(userRepo.Object, followRepo.Object);

        // Act
        var result = await handler.Handle(
            new GetUserByIdQuery(userId, requestingUserId), Ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<UserDto>();

        followRepo.Verify(r => r.UserFollowsAsync(requestingUserId, userId, Ct), Times.Once);
        userRepo.Verify(r => r.GetByIdAsync(userId, Ct), Times.Once);
    }
}
