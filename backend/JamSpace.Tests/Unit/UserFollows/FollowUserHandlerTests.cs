using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.UserFollows.Commands.FollowUser;
using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserFollows;

public class FollowUserHandlerTests
{
    private readonly Mock<IUserFollowRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Create_Follow_And_Map_Dto_When_Not_Already_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new FollowUserCommand(followerId, followeeId);

        _repo.Setup(r => r.UserFollowsAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        UserFollow? added = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
            .Callback<UserFollow, CancellationToken>((uf, _) => added = uf)
            .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new FollowUserHandler(_repo.Object, _uow.Object);

        // Act
        BasicUserFollowDto dto = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        dto.FollowerId.Should().Be(followerId);
        dto.FolloweeId.Should().Be(followeeId);

        added.Should().NotBeNull();
        added!.FollowerId.Should().Be(followerId);
        added.FolloweeId.Should().Be(followeeId);

        _repo.Verify(r => r.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Conflict_When_Already_Following()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();
        var cmd = new FollowUserCommand(followerId, followeeId);

        _repo.Setup(r => r.UserFollowsAsync(followerId, followeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new FollowUserHandler(_repo.Object, _uow.Object);

        // Act
        var act = () => sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("You are already following this user.");

        _repo.Verify(r => r.AddAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}