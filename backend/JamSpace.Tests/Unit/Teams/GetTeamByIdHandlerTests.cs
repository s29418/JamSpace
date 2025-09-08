using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Queries.GetTeamById;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.Teams;

public class GetTeamByIdHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_If_User_Not_In_Team()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        
        var creator = new User { UserName = "creator" };

        teamMemberRepo.Setup(r => r.IsUserInTeamAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        teamRepo.Setup(r => r.GetTeamByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Team
            {
                Id = Guid.NewGuid(),
                Name = "Test Team",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = creator,
                Members = new List<TeamMember>
                {
                    new TeamMember { User = creator }
                }
            });

        var handler = new GetTeamByIdHandler(teamRepo.Object, teamMemberRepo.Object);;

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new GetTeamByIdQuery(
                Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_Team_When_User_In_Team()
    {
        // Arrange
        var teamRepo = new Mock<ITeamRepository>();
        var teamMemberRepo = new Mock<ITeamMemberRepository>();
        
        var creator = new User { UserName = "creator" };

        teamMemberRepo.Setup(r => r.IsUserInTeamAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    
        teamRepo.Setup(r => r.GetTeamByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Team
            {
                Id = Guid.NewGuid(),
                Name = "Test Team",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = creator,
                Members = new List<TeamMember>
                {
                    new TeamMember { User = creator }
                }
            });
    
        var handler = new GetTeamByIdHandler(teamRepo.Object, teamMemberRepo.Object);
        
        // Act
        var result = await handler.Handle(new GetTeamByIdQuery(
            Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);
    
        // Assert
        result.Name.Should().Be("Test Team");
        result.CreatedByUserName.Should().Be("creator");
    }
}