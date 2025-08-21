using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class KickTeamMemberHandlerTests
{
    
    [Fact]
    public async Task Should_Kick_Member_When_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        var teamId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        
        repo.Setup(r => r.IsUserALeaderAsync(teamId, requestingUserId, CancellationToken.None))
            .ReturnsAsync(true);
        
        repo.Setup(r => r.IsUserALeaderAsync(teamId, targetUserId, CancellationToken.None))
            .ReturnsAsync(false);

        repo.Setup(r => r.GetTeamMemberAsync(
                teamId, targetUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamMember
            {
                Role = FunctionalRole.Member,
                User = new User { UserName = "member" }
            });

        var handler = new KickTeamMemberHandler(repo.Object);

        // Act
        await handler.Handle(
            new KickTeamMemberCommand(teamId, requestingUserId, targetUserId),
            CancellationToken.None
        );

        // Assert
        repo.Verify(r => r.DeleteTeamMemberAsync(
            teamId, targetUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    
    [Fact]
    public async Task Should_Throw_Conflict_When_Attempting_To_Kick_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();
        
        repo.Setup(r => r.IsUserALeaderAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        repo.Setup(r => r.GetTeamMemberAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamMember
            {
                Role = FunctionalRole.Leader,
                User = new User { UserName = "leader" }
            });

        var handler = new KickTeamMemberHandler(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new KickTeamMemberCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Not_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();
        repo.Setup(r => r.IsUserALeaderAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new KickTeamMemberHandler(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(
                new KickTeamMemberCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}