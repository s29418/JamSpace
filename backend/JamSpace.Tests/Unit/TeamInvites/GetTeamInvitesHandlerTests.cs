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
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Member, Ct)).ReturnsAsync(false);

        var handler = new GetTeamInvitesHandler(inviteRepo.Object, memberRepo.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new GetTeamInvitesQuery(teamId, userId), CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_Only_SentByUser_Invites_When_User_Is_Member()
    {
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Member, Ct)).ReturnsAsync(true);
        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Leader, Ct)).ReturnsAsync(false);
        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Admin, Ct)).ReturnsAsync(false);

        inviteRepo.Setup(r => r.GetPendingInvitesForTeamSentByUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamInvite>
            {
                new TeamInvite
                {
                    Status = InviteStatus.Pending,
                    Team = new Team { Id = teamId, Name = "Test team" },
                    InvitedByUser = new User { Id = userId, UserName = "Inviter", DisplayName = "Inviter" },
                    InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee", DisplayName = "Invitee" }
                }
            });

        var handler = new GetTeamInvitesHandler(inviteRepo.Object, memberRepo.Object);

        var result = await handler.Handle(new GetTeamInvitesQuery(teamId, userId), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(InviteStatus.Pending.ToString());

        inviteRepo.Verify(r => r.GetPendingInvitesForTeamSentByUserAsync(teamId, userId, It.IsAny<CancellationToken>()), Times.Once);
        inviteRepo.Verify(r => r.GetPendingInvitesForTeamAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Return_All_Invites_When_User_Is_Admin()
    {
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Member, Ct)).ReturnsAsync(true);
        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Admin, Ct)).ReturnsAsync(true);

        inviteRepo.Setup(r => r.GetPendingInvitesForTeamAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamInvite>
            {
                new TeamInvite
                {
                    Status = InviteStatus.Pending,
                    Team = new Team { Id = teamId, Name = "Test team" },
                    InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter", DisplayName = "Inviter" },
                    InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee", DisplayName = "Invitee" }
                }
            });

        var handler = new GetTeamInvitesHandler(inviteRepo.Object, memberRepo.Object);

        var result = await handler.Handle(new GetTeamInvitesQuery(teamId, userId), CancellationToken.None);

        result.Should().HaveCount(1);
        inviteRepo.Verify(r => r.GetPendingInvitesForTeamAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
    }
}