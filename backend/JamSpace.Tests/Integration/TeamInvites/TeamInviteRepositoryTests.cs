using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using JamSpace.Infrastructure.Data;
using JamSpace.Infrastructure.Repositories;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using Moq;

namespace JamSpace.Tests.Integration.TeamInvites;

public class TeamInviteRepositoryTests
{
    private JamSpaceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<JamSpaceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new JamSpaceDbContext(options);
    }

    [Fact]
    public async Task Should_Add_Invite()
    {
        // Arrange
        await using var db = CreateDbContext();
        var memberRepo = new Mock<ITeamMemberRepository>();
        memberRepo.Setup(r => r.IsUserInTeamAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var repo = new TeamInviteRepository(db, memberRepo.Object);

        // Act
        var invite = await repo.SendTeamInviteAsync(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        invite.Status.Should().Be(InviteStatus.Pending);
        (await db.TeamInvites.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Should_Accept_Invite()
    {
        // Arrange
        await using var db = CreateDbContext();
        
        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            InvitedUserId = Guid.NewGuid(),
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };

        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();

        var memberRepo = new Mock<ITeamMemberRepository>();
        var repo = new TeamInviteRepository(db, memberRepo.Object);

        // Act
        var result = await repo.AcceptInviteAsync(invite.Id, invite.InvitedUserId, CancellationToken.None);

        // Assert
        result.Status.Should().Be(InviteStatus.Accepted);
        (await db.TeamMembers.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Should_Reject_Invite()
    {
        // Arrange
        await using var db = CreateDbContext();
        var repo = new TeamInviteRepository(db, Mock.Of<ITeamMemberRepository>());
    
        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            InvitedUserId = Guid.NewGuid(),
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
    
        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();
    
        // Act
        var result = await repo.RejectInviteAsync(invite.Id, invite.InvitedUserId, CancellationToken.None);
    
        // Assert
        result.Status.Should().Be(InviteStatus.Rejected);
    }
    
    [Fact]
    public async Task Should_Cancel_Invite()
    {
        // Arrange
        await using var db = CreateDbContext();
        var repo = new TeamInviteRepository(db, Mock.Of<ITeamMemberRepository>());
    
        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            InvitedUserId = Guid.NewGuid(),
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        
        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();
    
        // Act
        var result = await repo.CancelTeamInviteAsync(invite.Id, invite.InvitedByUserId, CancellationToken.None);
    
        // Assert
        result.Status.Should().Be(InviteStatus.Cancelled);
    }
    
    [Fact]
    public async Task Should_Get_My_Pending_Invites()
    {
        // Arrange
        await using var db = CreateDbContext();
        var repo = new TeamInviteRepository(db, Mock.Of<ITeamMemberRepository>());
    
        var userId = Guid.NewGuid();
    
        
        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            InvitedUserId = userId,
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        
        var invite2 = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            InvitedUserId = userId,
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Accepted
        };
    
        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.AddRange(invite, invite2);
        await db.SaveChangesAsync();
    
        // Act
        var result = await repo.GetMyPendingInvitesAsync(userId, CancellationToken.None);
    
        // Assert
        result.Should().HaveCount(1);
        result[0].Status.Should().Be(InviteStatus.Pending);
    }
    
    [Fact]
    public async Task Should_Get_Team_Invites_As_Leader()
    {
        // Arrange
        await using var db = CreateDbContext();
        var memberRepo = new Mock<ITeamMemberRepository>();
        memberRepo.Setup(r => r.IsUserALeaderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);
        memberRepo.Setup(r => r.IsUserAnAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);
    
        var repo = new TeamInviteRepository(db, memberRepo.Object);
    
        var teamId = Guid.NewGuid();

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            InvitedUserId = Guid.NewGuid(),
            InvitedByUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        
        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();
    
        // Act
        var result = await repo.GetTeamInvitesAsync(teamId, Guid.NewGuid(), CancellationToken.None);
    
        // Assert
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task Should_Get_Team_Invites_As_User_Who_Sent_Them()
    {
        // Arrange
        await using var db = CreateDbContext();
        var memberRepo = new Mock<ITeamMemberRepository>();
        memberRepo.Setup(r => r.IsUserALeaderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);
        memberRepo.Setup(r => r.IsUserAnAdminAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);
    
        var repo = new TeamInviteRepository(db, memberRepo.Object);
    
        var teamId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            InvitedUserId = Guid.NewGuid(),
            InvitedByUserId = requestingUserId,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
    
        var team = new Team { Id = invite.TeamId, Name = "Test Team" };
        var invitedUser = new User { Id = invite.InvitedUserId, UserName = "invited", Email = "<EMAIL>", PasswordHash = "<PASSWORD>"};
        var invitedByUser = new User { Id = invite.InvitedByUserId, UserName = "inviter", Email = "<EMAIL2>", PasswordHash = "<PASSWORD2>" };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();
    
        // Act
        var result = await repo.GetTeamInvitesAsync(teamId, requestingUserId, CancellationToken.None);
    
        // Assert
        result.Should().HaveCount(1);
    }
}
