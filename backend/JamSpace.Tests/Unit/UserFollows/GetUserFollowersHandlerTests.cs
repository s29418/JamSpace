using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Application.Features.UserFollows.Queries.GetUserFollowers;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserFollows;

public class GetUserFollowersHandlerTests
{
    private readonly Mock<IUserFollowRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Map_Follower_Rows_To_Dtos()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var rows = new HashSet<UserFollow>
        {
            new() { FollowerId = Guid.NewGuid(), FolloweeId = userId },
            new() { FollowerId = Guid.NewGuid(), FolloweeId = userId }
        };

        _repo.Setup(r => r.GetFollowersAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows.ToHashSet());

        var sut = new GetUserFollowersHandler(_repo.Object);

        // Act
        var result = await sut.Handle(new GetUserFollowersQuery(userId, userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.FolloweeId == userId);
        result.ToList()[0].FollowerId.Should().Be(rows.ToList()[0].FollowerId);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Followers()
    {
        var userId = Guid.NewGuid();
        _repo.Setup(r => r.GetFollowersAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<UserFollow>());

        var sut = new GetUserFollowersHandler(_repo.Object);

        var result = await sut.Handle(new GetUserFollowersQuery(userId, userId), CancellationToken.None);

        result.Should().NotBeNull().And.BeEmpty();
    }
}