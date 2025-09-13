using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;

namespace JamSpace.Tests.Unit.UserFollows;

public class UnfollowUserHandlerTests
{
    private readonly Mock<IUserFollowRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Unfollow_When_Currently_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new UnfollowUserCommand(followerId, followeeId);

        _repo.Setup(r => r.UserFollowsAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        _repo.Setup(r => r.UnfollowUserAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var sut = new UnfollowUserHandler(_repo.Object);

        // Act
        MediatR.Unit result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        _repo.Verify(r => r.UnfollowUserAsync(
            followerId, followeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_Not_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new UnfollowUserCommand(followerId, followeeId);

        _repo.Setup(r => r.UserFollowsAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        var sut = new UnfollowUserHandler(_repo.Object);

        // Act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
                 .WithMessage("You are not following this user.");
        _repo.Verify(r => r.UnfollowUserAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
