using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Queries.GetMyTeams;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Teams;

public class GetMyTeamsHandlerTests
{
    [Fact]
    public async Task Should_Return_Teams()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();

        var creator = new User { UserName = "creator", DisplayName = "creator" };

        teamRepo.Setup(r => r.GetTeamsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>
            {
                new Team
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
                }
            });

        var handler = new GetMyTeamsHandler(teamRepo.Object);

        // Act
        var result = await handler.Handle(new GetMyTeamsQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test Team");
        result[0].CreatedByUserName.Should().Be("creator");
    }
}