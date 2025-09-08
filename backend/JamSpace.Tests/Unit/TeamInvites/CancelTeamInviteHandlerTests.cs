using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.Commands.CancelTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class CancelTeamInviteHandlerTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Has_No_Permission()
    {
        // Arrange
        var inviteId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        

        var inviteRepo = new Mock<ITeamInviteRepository>();
        inviteRepo.Setup(r => r.GetTeamInviteByIdAsync(
                inviteId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new TeamInvite { TeamId = teamId });

        var memberRepo = new Mock<ITeamMemberRepository>();
        memberRepo.Setup(r => r.IsUserALeaderAsync(teamId, userId, Ct)).ReturnsAsync(false);
        memberRepo.Setup(r => r.IsUserAnAdminAsync(teamId, userId, Ct)).ReturnsAsync(false);
        inviteRepo.Setup(r => r.WasInviteSentByUserAsync(
            teamId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new CancelTeamInviteHandler(inviteRepo.Object, memberRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new CancelTeamInviteCommand(inviteId, userId), CancellationToken.None));
    }

    [Fact]
    public async Task Should_Cancel_Invite_When_User_Has_Permission()
    {
        // Arrange
        var inviteId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        inviteRepo.Setup(r => r.GetTeamInviteByIdAsync(
                inviteId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new TeamInvite { TeamId = teamId });
        inviteRepo.Setup(r => r.CancelTeamInviteAsync(
                inviteId, userId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new TeamInvite
                  {
                      Status = InviteStatus.Cancelled,
                      Team = new Team { Id = Guid.NewGuid(), Name = "Test team" },
                      InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter" },
                      InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee" }
                  });

        var memberRepo = new Mock<ITeamMemberRepository>();
        memberRepo.Setup(r => r.IsUserALeaderAsync(teamId, userId, Ct)).ReturnsAsync(true);

        var handler = new CancelTeamInviteHandler(inviteRepo.Object, memberRepo.Object);

        // Act
        var result = await handler.Handle(
            new CancelTeamInviteCommand(inviteId, userId), CancellationToken.None);

        // Assert
        result.Status.Should().Be(InviteStatus.Cancelled.ToString());
    }
}
