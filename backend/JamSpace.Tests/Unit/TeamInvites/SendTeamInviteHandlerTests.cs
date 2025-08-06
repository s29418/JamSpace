using FluentAssertions;
using Moq;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.Commands.SendTeamInvite;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Tests.Unit.TeamInvites;

public class SendTeamInviteHandlerTests
{
    [Fact]
    public async Task Should_Throw_Forbidden_When_User_Not_Member()
    {
        // Arrange 
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();

        memberRepo.Setup(r => r.IsUserInTeamAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object);

        var command = new SendTeamInviteCommand("testuser", Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_User_Not_Exists()
    {
        // Arrange
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();

        memberRepo.Setup(r => r.IsUserInTeamAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
        userRepo.Setup(r => r.GetUserIdByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid?)null);

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object);

        var command = new SendTeamInviteCommand("nonexistent", Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Send_Invite_When_Valid()
    {
        // Arrange
        var inviteRepo = new Mock<ITeamInviteRepository>();
        var memberRepo = new Mock<ITeamMemberRepository>();
        var userRepo = new Mock<IUserRepository>();

        memberRepo.Setup(r => r.IsUserInTeamAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        userRepo.Setup(r => r.GetUserIdByUsernameAsync(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());
        inviteRepo.Setup(r => r.SendTeamInviteAsync(
                It.IsAny<Guid>(), 
                It.IsAny<Guid>(), 
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamInvite
            {
                Status = InviteStatus.Pending,
                Team = new Team { Id = Guid.NewGuid(), Name = "Test team" },
                InvitedByUser = new User { Id = Guid.NewGuid(), UserName = "Inviter" },
                InvitedUser = new User { Id = Guid.NewGuid(), UserName = "Invitee" }
            });

        var handler = new SendTeamInviteHandler(inviteRepo.Object, memberRepo.Object, userRepo.Object);

        var command = new SendTeamInviteCommand("validuser", Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(InviteStatus.Pending.ToString());
    }
}
