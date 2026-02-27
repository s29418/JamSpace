using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Teams.Commands.DeleteTeam;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Teams;

public class DeleteTeamHandlerTests
{
    [Fact]
    public async Task Should_Delete_Team_When_User_Is_Leader()
    {
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var team = new Team { Id = teamId, Name = "x" };
        teamRepo.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var handler = new DeleteTeamHandler(teamRepo.Object, teamMemberRepo.Object, uow.Object);

        await handler.Handle(new DeleteTeamCommand(teamId, userId), CancellationToken.None);

        teamRepo.Verify(r => r.Remove(team), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Is_Not_Leader()
    {
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteTeamHandler(teamRepo.Object, teamMemberRepo.Object, uow.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new DeleteTeamCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}