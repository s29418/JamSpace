using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Teams;

public class ChangeTeamNameHandlerTests
{
    [Fact]
    public async Task Should_Change_Team_Name_When_User_Has_Permission()
    {
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var team = new Team
        {
            Id = teamId,
            Name = "Old",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = new User { UserName = "creator", DisplayName = "creator" },
            Members = new List<TeamMember>()
        };

        teamRepo.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var handler = new ChangeTeamNameHandler(teamRepo.Object, teamMemberRepo.Object, uow.Object);

        var result = await handler.Handle(new ChangeTeamNameCommand(teamId, userId, "New Name"), CancellationToken.None);

        result.Name.Should().Be("New Name");
        team.Name.Should().Be("New Name");
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Has_No_Permission()
    {
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        var uow = new Mock<IUnitOfWork>();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ChangeTeamNameHandler(teamRepo.Object, teamMemberRepo.Object, uow.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new ChangeTeamNameCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name"), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}