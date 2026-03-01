using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
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
        var inviteId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        inviteRepo.Setup(r => r.GetByIdWithDetailsAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite { Id = inviteId, TeamId = teamId, Status = InviteStatus.Pending });

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Admin, Ct)).ReturnsAsync(false);
        inviteRepo.Setup(r => r.WasInviteSentByUserAsync(inviteId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new CancelTeamInviteHandler(inviteRepo.Object, memberRepo.Object, uow.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new CancelTeamInviteCommand(inviteId, userId), CancellationToken.None));
    }

    [Fact]
    public async Task Should_Cancel_Invite_When_User_Is_Admin()
    {
        var inviteId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        inviteRepo.Setup(r => r.GetByIdWithDetailsAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite
            {
                Id = inviteId,
                TeamId = teamId,
                Status = InviteStatus.Pending,

                Team = new Team
                {
                    Id = teamId,
                    Name = "Test team"
                },
                InvitedByUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "Inviter", 
                    DisplayName = "Inviter"
                },
                InvitedUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "Invitee",
                    DisplayName = "Invitee"
                }
            });

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CancelTeamInviteHandler(inviteRepo.Object, memberRepo.Object, uow.Object);

        var result = await handler.Handle(new CancelTeamInviteCommand(inviteId, userId), CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Cancelled.ToString());
        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}