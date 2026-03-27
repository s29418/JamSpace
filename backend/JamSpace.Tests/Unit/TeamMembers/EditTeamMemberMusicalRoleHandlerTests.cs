using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.Commands.EditTeamMemberMusicalRole;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class EditTeamMemberMusicalRoleHandlerTests
{
    [Fact]
    public async Task Should_Edit_MusicalRole_When_RequestingUser_Is_AdminOrLeader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, adminId, FunctionalRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = new User { Id = userId, UserName = "x", DisplayName = "x" };
        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            Role = FunctionalRole.Member,
            MusicalRole = "Old",
            User = user
        };

        repo.Setup(r => r.GetByTeamAndUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var participant = new ConversationParticipant
        {
            ConversationId  = Guid.NewGuid(),
            UserId = userId,
            Role = "Old"
        };

        conversationParticipantRepo
            .Setup(r => r.GetAsync(teamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participant);

        var handler = new EditTeamMemberMusicalRoleHandler(repo.Object, uow.Object, conversationParticipantRepo.Object);

        var result = await handler.Handle(
            new EditTeamMemberMusicalRoleCommand(teamId, adminId, userId, "New"),
            CancellationToken.None);

        member.MusicalRole.Should().Be("New");
        participant.Role.Should().Be("New");
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.MusicalRole.Should().Be("New");
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_RequestingUser_Is_Not_AdminOrLeader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.HasRequiredRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new EditTeamMemberMusicalRoleHandler(repo.Object, uow.Object, conversationParticipantRepo.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new EditTeamMemberMusicalRoleCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "New"
            ), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}