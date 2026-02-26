using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Commands.ChangeTeamName;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.Teams;

public class ChangeTeamNameHandlerTests
{
    [Fact]
    public async Task Should_Change_Team_Name_When_User_Is_Leader()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        var creator = new User { UserName = "creator", DisplayName = "creator"};

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        teamRepo.Setup(r => r.ChangeTeamNameAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Team
            {
                Id = Guid.NewGuid(),
                Name = "New Name",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = creator,
                Members = new List<TeamMember>
                {
                    new TeamMember
                    {
                        User = creator
                    }
                }
            });

        var handler = new ChangeTeamNameHandler(teamRepo.Object, teamMemberRepo.Object);

        // Act
        var result = await handler.Handle(
            new ChangeTeamNameCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name"), 
            CancellationToken.None);

        // Assert
        result.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Has_No_Permission()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();

        teamMemberRepo.Setup(r => r.HasRequiredRoleAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Admin, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        
        var handler = new ChangeTeamNameHandler(teamRepo.Object, teamMemberRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new ChangeTeamNameCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name"), 
                CancellationToken.None));
    }
}
