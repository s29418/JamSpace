using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class RejectTeamInviteHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Invited()
    {
        var repo = new Mock<ITeamInviteRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite { InvitedUserId = Guid.NewGuid(), Status = InviteStatus.Pending });

        var handler = new RejectTeamInviteHandler(repo.Object, uow.Object);
        var command = new RejectTeamInviteCommand(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Reject_Invite_When_User_Is_Invited()
    {
        var inviteId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var repo = new Mock<ITeamInviteRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdWithDetailsAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite
            {
                Id = inviteId,
                InvitedUserId = userId,
                Status = InviteStatus.Pending,
                Team = new Team { Id = Guid.NewGuid(), Name = "Test team" },
                InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter", DisplayName = "Inviter" },
                InvitedUser = new User { Id = userId, UserName = "Invitee", DisplayName = "Invitee" }
            });

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RejectTeamInviteHandler(repo.Object, uow.Object);

        var result = await handler.Handle(new RejectTeamInviteCommand(inviteId, userId), CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Rejected.ToString());
        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}