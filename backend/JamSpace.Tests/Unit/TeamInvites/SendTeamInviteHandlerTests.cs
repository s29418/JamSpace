using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class SendTeamInviteHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Member()
    {
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        memberRepo.Setup(r => r.HasRequiredRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Member, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object, uow.Object);
        var command = new SendTeamInviteCommand("testuser", Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Exists()
    {
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        memberRepo.Setup(r => r.HasRequiredRoleAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), FunctionalRole.Member, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        userRepo.Setup(r => r.GetUserIdByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object, uow.Object);
        var command = new SendTeamInviteCommand("nonexistent", Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Send_Invite_When_Valid()
    {
        var teamId = Guid.NewGuid();
        var invitingUserId = Guid.NewGuid();
        var invitedUserId = Guid.NewGuid();

        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();

        // inviter jest memberem
        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, invitingUserId, FunctionalRole.Member, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // invited user nie jest w teamie (czyli "alreadyInTeam" = false)
        memberRepo.Setup(r => r.HasRequiredRoleAsync(teamId, invitedUserId, FunctionalRole.Member, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        userRepo.Setup(r => r.GetUserIdByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invitedUserId);

        inviteRepo.Setup(r => r.ExistsPendingInviteAsync(teamId, invitedUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        TeamInvite? addedInvite = null;
        inviteRepo.Setup(r => r.AddAsync(It.IsAny<TeamInvite>(), It.IsAny<CancellationToken>()))
            .Callback<TeamInvite, CancellationToken>((i, _) => addedInvite = i)
            .Returns(Task.CompletedTask);

        uow.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        inviteRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TeamInvite
            {
                Id = addedInvite?.Id ?? Guid.NewGuid(),
                Status = InviteStatus.Pending,
                Team = new Team { Id = teamId, Name = "Test team" },
                InvitedByUser = new User { Id = invitingUserId, UserName = "Inviter", DisplayName = "Inviter" },
                InvitedUser = new User { Id = invitedUserId, UserName = "Invitee", DisplayName = "Invitee" }
            });

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object, uow.Object);
        var command = new SendTeamInviteCommand("validuser", teamId, invitingUserId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be(InviteStatus.Pending.ToString());

        inviteRepo.Verify(r => r.AddAsync(It.IsAny<TeamInvite>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        inviteRepo.Verify(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}