using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.Queries.GetMyPendingInvites;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class GetMyPendingInvitesHandlerTests
{
    [Fact]
    public async Task Should_Return_Pending_Invites()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var repo = new Mock<ITeamInviteRepository>();
        repo.Setup(r => r.GetMyPendingInvitesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamInvite>
            {
                new TeamInvite
                {
                    Status = InviteStatus.Pending,
                    Team = new Team { Id = Guid.NewGuid(), Name = "Test team" },
                    InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter" },
                    InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee" }
                }
            });

        var handler = new GetMyPendingInvitesHandler(repo.Object);

        // Act
        var result = await handler.Handle(new GetMyPendingInvitesQuery(userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Status.Should().Be(InviteStatus.Pending.ToString());
    }
}