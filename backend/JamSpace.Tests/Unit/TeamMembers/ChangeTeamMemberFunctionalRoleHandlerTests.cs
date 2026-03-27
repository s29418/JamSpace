using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class ChangeTeamMemberFunctionalRoleHandlerTests
{
    [Fact]
    public async Task Should_Change_Role_When_RequestingUser_Is_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var leaderId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, leaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = new User { Id = memberId, UserName = "member", DisplayName = "member" };
        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = memberId,
            Role = FunctionalRole.Member,
            MusicalRole = "Guitar",
            User = user
        };

        repo.Setup(r => r.GetByTeamAndUserAsync(teamId, memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var handler = new ChangeTeamMemberFunctionalRoleHandler(repo.Object, uow.Object);

        var result = await handler.Handle(new ChangeTeamMemberFunctionalRoleCommand(
            teamId, leaderId, memberId, FunctionalRole.Admin
        ), CancellationToken.None);

        member.Role.Should().Be(FunctionalRole.Admin);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.UserId.Should().Be(memberId);
        result.Role.Should().Be(FunctionalRole.Admin.ToString());
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_RequestingUser_Is_Not_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.HasRequiredRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ChangeTeamMemberFunctionalRoleHandler(repo.Object, uow.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new ChangeTeamMemberFunctionalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FunctionalRole.Admin
            ), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Conflict_When_Demoting_Last_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var leaderId = Guid.NewGuid();

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, leaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var leaderUser = new User { Id = leaderId, UserName = "leader", DisplayName = "leader" };
        var leaderMember = new TeamMember
        {
            TeamId = teamId,
            UserId = leaderId,
            Role = FunctionalRole.Leader,
            User = leaderUser
        };

        repo.Setup(r => r.GetByTeamAndUserAsync(teamId, leaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leaderMember);

        repo.Setup(r => r.GetLeadersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamMember> { leaderMember }); // last leader

        var handler = new ChangeTeamMemberFunctionalRoleHandler(repo.Object, uow.Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new ChangeTeamMemberFunctionalRoleCommand(
                teamId, leaderId, leaderId, FunctionalRole.Admin
            ), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}