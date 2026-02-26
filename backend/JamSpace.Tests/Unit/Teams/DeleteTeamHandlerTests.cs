using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Teams;

public class DeleteTeamHandlerTests
{
    [Fact]
    public async Task Should_Delete_Team_When_User_Is_Leader()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteTeamHandler(teamRepo.Object, teamMemberRepo.Object);

        // Act
        await handler.Handle(new DeleteTeamCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        teamRepo.Verify(r => r.DeleteTeamAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Leader()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteTeamHandler(teamRepo.Object, teamMemberRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new DeleteTeamCommand(Guid.NewGuid(), Guid.NewGuid()), 
                CancellationToken.None));
    }
}