using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.Queries.GetTeamInvites;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class GetTeamInvitesHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_In_Team()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();

        memberRepo.Setup(r => r.IsUserInTeamAsync(teamId, userId, Ct)).ReturnsAsync(false);

        var handler = new GetTeamInvitesHandler(inviteRepo.Object, memberRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new GetTeamInvitesQuery(teamId, userId), CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_Invites_When_User_In_Team()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();

        memberRepo.Setup(r => r.IsUserInTeamAsync(teamId, userId, Ct)).ReturnsAsync(true);
        inviteRepo.Setup(r => r.GetTeamInvitesAsync(
                teamId, userId, It.IsAny<CancellationToken>()))
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

        var handler = new GetTeamInvitesHandler(inviteRepo.Object, memberRepo.Object);

        // Act
        var result = await handler.Handle(
            new GetTeamInvitesQuery(teamId, userId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Status.Should().Be(InviteStatus.Pending.ToString());
    }
}
