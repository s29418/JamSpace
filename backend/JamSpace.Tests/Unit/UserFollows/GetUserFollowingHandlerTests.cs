using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.UserFollows.DTOs;
using JamSpace.Application.Features.UserFollows.Queries.GetUserFollowing;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.UserFollows;

public class GetUserFollowingHandlerTests
{
    private readonly Mock<IUserFollowRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Map_Following_Rows_To_Dtos()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var rows = new List<UserFollow>
        {
            new() { FollowerId = userId, FolloweeId = Guid.NewGuid() },
            new() { FollowerId = userId, FolloweeId = Guid.NewGuid() }
        };

        _repo.Setup(r => r.GetFollowingAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var sut = new GetUserFollowingHandler(_repo.Object);

        // Act
        List<DetailedUserFollowDto> result = await sut.Handle(new GetUserFollowingQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.FollowerId == userId);
        result[0].FolloweeId.Should().Be(rows[0].FolloweeId);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_Not_Following_Anyone()
    {
        var userId = Guid.NewGuid();

        _repo.Setup(r => r.GetFollowingAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserFollow>());

        var sut = new GetUserFollowingHandler(_repo.Object);

        var result = await sut.Handle(new GetUserFollowingQuery(userId), CancellationToken.None);

        result.Should().NotBeNull().And.BeEmpty();
    }
}