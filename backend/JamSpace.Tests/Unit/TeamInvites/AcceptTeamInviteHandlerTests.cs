using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class AcceptTeamInviteHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Invited()
    {
        // Arrange
        var repo = new Mock<ITeamInviteRepository>();
        repo.Setup(r => r.GetTeamInviteByIdAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite { InvitedUserId = Guid.NewGuid() });

        var handler = new AcceptTeamInviteHandler(repo.Object, Mock.Of<ITeamMemberRepository>());

        var command = new AcceptTeamInviteCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Accept_Invite_When_User_Is_Invited()
    {
        // Arrange
        var inviteId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var repo = new Mock<ITeamInviteRepository>();
        repo.Setup(r => r.GetTeamInviteByIdAsync(
                inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite { InvitedUserId = userId });
        
        repo.Setup(r => r.AcceptInviteAsync(
                inviteId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite
            {
                Status = InviteStatus.Accepted,
                Team = new Team { Id = Guid.NewGuid(), Name = "Test team" },
                InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter", DisplayName = "Inviter" },
                InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee", DisplayName = "Invitee" }
            });

        var handler = new AcceptTeamInviteHandler(
            repo.Object, Mock.Of<ITeamMemberRepository>());

        var command = new AcceptTeamInviteCommand(inviteId, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(InviteStatus.Accepted.ToString());
    }
}