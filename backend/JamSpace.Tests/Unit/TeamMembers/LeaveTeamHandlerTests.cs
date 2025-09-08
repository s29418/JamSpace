using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.Commands.LeaveTeam;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class LeaveTeamHandlerTests
{
    [Fact]
    public async Task Should_Leave_Team_When_Not_Last_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        repo.Setup(r => r.GetLeadersAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamMember>
            {
                new TeamMember { UserId = Guid.NewGuid(), Role = FunctionalRole.Leader },
                new TeamMember { UserId = Guid.NewGuid(), Role = FunctionalRole.Leader }
            });

        var handler = new LeaveTeamHandler(repo.Object);

        // Act
        await handler.Handle(
            new LeaveTeamCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None
        );

        // Assert
        repo.Verify(r => r.DeleteTeamMemberAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Conflict_When_Last_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        repo.Setup(r => r.GetLeadersAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TeamMember>
            {
                new TeamMember { UserId = Guid.NewGuid(), Role = FunctionalRole.Leader }
            });

        repo.Setup(r => r.IsUserALeaderAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new LeaveTeamHandler(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new LeaveTeamCommand(Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}
