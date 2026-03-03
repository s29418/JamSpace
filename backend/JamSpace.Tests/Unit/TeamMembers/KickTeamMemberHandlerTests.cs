using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamMembers.Commands.KickTeamMember;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamMembers;

public class KickTeamMemberHandlerTests
{
    [Fact]
    public async Task Should_Kick_Member_When_RequestingUser_Is_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var leaderId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, leaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, memberId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = memberId,
            Role = FunctionalRole.Member,
            User = new User { Id = memberId, UserName = "m", DisplayName = "m" }
        };

        repo.Setup(r => r.GetByTeamAndUserAsync(teamId, memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var participant = new ConversationParticipant { ConversationId = Guid.NewGuid(), UserId = memberId };
        conversationParticipantRepo
            .Setup(r => r.GetByUserAndTeamAsync(teamId, memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participant);

        var handler = new KickTeamMemberHandler(repo.Object, uow.Object, conversationParticipantRepo.Object);

        await handler.Handle(new KickTeamMemberCommand(teamId, leaderId, memberId), CancellationToken.None);

        repo.Verify(r => r.Remove(member), Times.Once);
        conversationParticipantRepo.Verify(r => r.Remove(participant), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Conflict_When_Trying_To_Kick_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        var teamId = Guid.NewGuid();
        var requestingLeaderId = Guid.NewGuid();
        var targetLeaderId = Guid.NewGuid();

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, requestingLeaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repo.Setup(r => r.HasRequiredRoleAsync(teamId, targetLeaderId, FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new KickTeamMemberHandler(repo.Object, uow.Object, conversationParticipantRepo.Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new KickTeamMemberCommand(teamId, requestingLeaderId, targetLeaderId), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        conversationParticipantRepo.Verify(x => x.Remove(It.IsAny<ConversationParticipant>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_RequestingUser_Is_Not_Leader()
    {
        var repo = new Mock<ITeamMemberRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.HasRequiredRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Leader, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new KickTeamMemberHandler(repo.Object, uow.Object, conversationParticipantRepo.Object);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new KickTeamMemberCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));

        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        conversationParticipantRepo.Verify(x => x.Remove(It.IsAny<ConversationParticipant>()), Times.Never);
    }
}