using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Commands.CreateTeam;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Teams;

public class CreateTeamHandlerTests
{
    [Fact]
    public async Task Should_Create_Team_And_Return_Dto()
    {
        // Arrange
        var repo = new Mock<ITeamRepository>();
        var createdTeamId = Guid.NewGuid();
        var creator = new User { UserName = "creator" };

        repo.Setup(r => r.CreateTeamAsync(It.IsAny<Team>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTeamId);

        repo.Setup(r => r.GetTeamByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Team
            {
                Id = Guid.NewGuid(),
                Name = "Test Team",
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


        var handler = new CreateTeamHandler(repo.Object);

        var command = new CreateTeamWithUserCommand(
            new CreateTeamCommand("Test Team", null),
            Guid.NewGuid()
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Team");
    }

}