using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class LeaveTeamHandlerTests
{
    [Fact]
    public async Task Should_Leave_Team_When_Not_Last_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        repo.Setup(r => r.GetLeadersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamMember>
            {
                new TeamMember
                {
                    TeamId = teamId,
                    UserId = Guid.NewGuid(),
                    Role = FunctionalRole.Leader
                }
            });
        
        repo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            Role = FunctionalRole.Member,
            User = new User
            {
                Id = userId,
                UserName = "u",
                DisplayName = "u"
            }
        };

        repo.Setup(r => r.GetByTeamAndUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var handler = new LeaveTeamHandler(repo.Object, uow.Object);

        await handler.Handle(new LeaveTeamCommand(teamId, userId), CancellationToken.None);

        repo.Verify(r => r.Remove(member), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Conflict_When_Last_Leader_Tries_To_Leave()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var leaderId = Guid.NewGuid();

        // only one leader exists
        repo.Setup(r => r.GetLeadersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamMember>
            {
                new TeamMember { TeamId = teamId, UserId = leaderId, Role = FunctionalRole.Leader }
            });

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, leaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new LeaveTeamHandler(repo.Object, uow.Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new LeaveTeamCommand(teamId, leaderId), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}