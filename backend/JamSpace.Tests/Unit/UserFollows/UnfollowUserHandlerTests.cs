using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserFollows.Commands.UnfollowUser;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserFollows;

public class UnfollowUserHandlerTests
{
    private readonly Mock<IUserFollowRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Unfollow_When_Currently_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new UnfollowUserCommand(followerId, followeeId);

        var existing = new UserFollow { FollowerId = followerId, FolloweeId = followeeId };

        _repo.Setup(r => r.GetAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new UnfollowUserHandler(_repo.Object, _uow.Object);

        // Act
        MediatR.Unit result = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);

        _repo.Verify(r => r.Remove(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_Not_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new UnfollowUserCommand(followerId, followeeId);

        _repo.Setup(r => r.GetAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFollow?)null);

        var sut = new UnfollowUserHandler(_repo.Object, _uow.Object);

        // Act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("You are not following this user.");

        _repo.Verify(r => r.Remove(It.IsAny<UserFollow>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}