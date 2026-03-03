using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class AcceptTeamInviteHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Invited()
    {
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var conversationRepo = new Mock<IConversationRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        inviteRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite { InvitedUserId = Guid.NewGuid(), Status = InviteStatus.Pending });

        var handler = new AcceptTeamInviteHandler(
            inviteRepo.Object,
            memberRepo.Object,
            uow.Object,
            conversationParticipantRepo.Object,
            conversationRepo.Object);

        var command = new AcceptTeamInviteCommand(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Accept_Invite_When_User_Is_Invited()
    {
        var inviteId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var conversationRepo = new Mock<IConversationRepository>();
        var conversationParticipantRepo = new Mock<IConversationParticipantRepository>();
        var uow = new Mock<IUnitOfWork>();

        inviteRepo.Setup(r => r.GetByIdWithDetailsAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite
            {
                Id = inviteId,
                TeamId = teamId,
                InvitedUserId = userId,
                Status = InviteStatus.Pending,
                Team = new Team { Id = teamId, Name = "Test team" },
                InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter", DisplayName = "Inviter" },
                InvitedUser = new User { Id = userId, UserName = "Invitee", DisplayName = "Invitee" }
            });

        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, userId, FunctionalRole.Member, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        memberRepo.Setup(r => r.AddAsync(It.IsAny<TeamMember>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var conversation = new Conversation { Id = conversationId, TeamId = teamId, Type = ChatType.Team};
        conversationRepo.Setup(r => r.GetForTeam(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        conversationParticipantRepo
            .Setup(r => r.AddAsync(It.IsAny<ConversationParticipant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AcceptTeamInviteHandler(
            inviteRepo.Object,
            memberRepo.Object,
            uow.Object,
            conversationParticipantRepo.Object,
            conversationRepo.Object);

        var result = await handler.Handle(new AcceptTeamInviteCommand(inviteId, userId), CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Accepted.ToString());

        memberRepo.Verify(r => r.AddAsync(
            It.Is<TeamMember>(tm => tm.TeamId == teamId && tm.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);

        conversationParticipantRepo.Verify(r => r.AddAsync(
            It.Is<ConversationParticipant>(cp => cp.ConversationId == conversationId && cp.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}