using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamMembers.Commands.ChangeTeamMemberFunctionalRole;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class ChangeTeamMemberFunctionalRoleHandlerTests
{
    [Fact]
    public async Task Should_Change_Functional_Role_When_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();

        repo.Setup(r => r.HasRequiredRoleAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<FunctionalRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repo.Setup(r => r.ChangeTeamMemberFunctionalRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), 
                It.IsAny<FunctionalRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamMember
            {
                User = new User { UserName = "member", DisplayName = "member" },
                Role = FunctionalRole.Admin
            });

        var handler = new ChangeTeamMemberFunctionalRoleHandler(repo.Object);

        // Act
        var result = await handler.Handle(
            new ChangeTeamMemberFunctionalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FunctionalRole.Admin),
            CancellationToken.None
        );

        // Assert
        result.Role.Should().Be(FunctionalRole.Admin.ToString());
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_Not_Leader()
    {
        // Arrange
        var repo = new Mock<ITeamMemberRepository>();
        repo.Setup(r => r.HasRequiredRoleAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ChangeTeamMemberFunctionalRoleHandler(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(
                new ChangeTeamMemberFunctionalRoleCommand(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FunctionalRole.Admin),
                CancellationToken.None));
    }
}
