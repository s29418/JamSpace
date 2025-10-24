using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;
using JamSpace.Domain.Entities;

namespace JamSpace.Tests.Unit.TeamMembers;

public class EditTeamMemberMusicalRoleHandlerTests
{
    [Fact]
    public async Task Should_Edit_Musical_Role_When_Leader_Or_Admin()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        repo.Setup(r => r.IsUserALeaderAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repo.Setup(r => r.EditTeamMemberMusicalRole(
                It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamMember
            {
                User = new User { UserName = "member", DisplayName = "member" },
                MusicalRole = "Guitarist"
            });

        var handler = new EditTeamMemberMusicalRoleHandler(repo.Object);

        // Act
        var result = await handler.Handle(
            new EditTeamMemberMusicalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Guitarist"),
            CancellationToken.None
        );

        // Assert
        result.MusicalRole.Should().Be("Guitarist");
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_No_Permission()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        repo.Setup(r => r.IsUserALeaderAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        repo.Setup(r => r.IsUserAnAdminAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new EditTeamMemberMusicalRoleHandler(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(
                new EditTeamMemberMusicalRoleCommand(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Guitarist"),
                CancellationToken.None));
    }
}
