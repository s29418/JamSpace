using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using JamSpace.Infrastructure.Data;
using JamSpace.Infrastructure.Repositories;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using System.Collections.Concurrent;
using JamSpace.Application.Common.Exceptions;

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
    
    private class TestTeamMemberRepository : ITeamMemberRepository
    {
        private readonly JamSpaceDbContext _db;

        private readonly ConcurrentDictionary<(Guid teamId, Guid userId), bool> _leaders = new();

        public TestTeamMemberRepository(JamSpaceDbContext db)
        {
            _db = db;
        }

        public void MarkLeader(Guid teamId, Guid userId) => _leaders[(teamId, userId)] = true;

        public Task<bool> IsUserInTeamAsync(Guid teamId, Guid userId, CancellationToken ct)
            => Task.FromResult(_db.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == userId));

        public Task<bool> IsUserALeaderAsync(Guid teamId, Guid userId, CancellationToken ct)
            => Task.FromResult(_leaders.ContainsKey((teamId, userId)));

        public Task<bool> IsUserAnAdminAsync(Guid teamId, Guid userId, CancellationToken ct)
        {
            var isAdmin = _db.TeamMembers
                .Any(m => m.TeamId == teamId 
                          && m.UserId == userId 
                          && (m.Role == FunctionalRole.Leader || m.Role == FunctionalRole.Admin));
            return Task.FromResult(isAdmin);
        }

        public Task<TeamMember> GetTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<TeamMember>> GetLeadersAsync(Guid teamId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<TeamMember> ChangeTeamMemberFunctionalRoleAsync(Guid teamId, Guid userId, FunctionalRole newRole, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<TeamMember> EditTeamMemberMusicalRole(Guid teamId, Guid userId, string musicalRole, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTeamMemberAsync(Guid teamId, Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
    }

    private static (User invited, User inviter, Team team) SeedUsersAndTeam(
        JamSpaceDbContext db, Guid? teamId = null, Guid? invitedId = null, Guid? inviterId = null)
    {
        var team = new Team
        {
            Id = teamId ?? Guid.NewGuid(), 
            Name = "Test Team"
        };
        var invitedUser = new User
        {
            Id = invitedId ?? Guid.NewGuid(), 
            UserName = "invited", 
            DisplayName = "Invited User",
            Email = "invited@test", 
            PasswordHash = "x"
        };
        var invitedByUser = new User
        {
            Id = inviterId ?? Guid.NewGuid(), 
            UserName = "inviter", 
            DisplayName = "Inviter User",
            Email = "inviter@test", 
            PasswordHash = "y"
        };

        db.Teams.Add(team);
        db.Users.AddRange(invitedUser, invitedByUser);
        db.SaveChanges();
        return (invitedUser, invitedByUser, team);
    }

    [Fact]
    public async Task Should_Add_Invite()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);

        var invite = await repo.SendTeamInviteAsync(team.Id, invited.Id, inviter.Id, CancellationToken.None);

        invite.Status.Should().Be(InviteStatus.Pending);
        (await db.TeamInvites.CountAsync()).Should().Be(1);
        invite.Team.Should().NotBeNull();
        invite.InvitedUser.Should().NotBeNull();
        invite.InvitedByUser.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Not_Add_Invite_When_Pending_Exists_Or_User_In_Team()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);
        
        db.TeamInvites.Add(new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = inviter.Id,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        });
        await db.SaveChangesAsync();

        Func<Task> duplicate = () => repo.SendTeamInviteAsync(team.Id, invited.Id, inviter.Id, CancellationToken.None);
        await duplicate.Should().ThrowAsync<ConflictException>();
        
        db.TeamInvites.RemoveRange(db.TeamInvites); 
        await db.TeamMembers.AddAsync(new TeamMember { TeamId = team.Id, UserId = invited.Id });
        await db.SaveChangesAsync();

        Func<Task> alreadyInTeam = () => repo.SendTeamInviteAsync(team.Id, invited.Id, inviter.Id, CancellationToken.None);
        await alreadyInTeam.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Should_Accept_Invite()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = inviter.Id,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();

        var result = await repo.AcceptInviteAsync(invite.Id, invited.Id, CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Accepted);
        (await db.TeamMembers.CountAsync()).Should().Be(1);
        db.TeamMembers.Single().UserId.Should().Be(invited.Id);
        db.TeamMembers.Single().TeamId.Should().Be(team.Id);
    }

    [Fact]
    public async Task Should_Reject_Invite()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = inviter.Id,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();

        var result = await repo.RejectInviteAsync(invite.Id, invited.Id, CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Rejected);
        (await db.TeamMembers.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Should_Cancel_Invite()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);

        var invite = new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = inviter.Id,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        };
        db.TeamInvites.Add(invite);
        await db.SaveChangesAsync();

        var result = await repo.CancelTeamInviteAsync(invite.Id, inviter.Id, CancellationToken.None);

        result.Status.Should().Be(InviteStatus.Cancelled);
    }

    [Fact]
    public async Task Should_Get_My_Pending_Invites()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);

        db.TeamInvites.AddRange(
            new TeamInvite
            {
                Id = Guid.NewGuid(),
                TeamId = team.Id,
                InvitedUserId = invited.Id,
                InvitedByUserId = inviter.Id,
                CreatedAt = DateTime.UtcNow,
                Status = InviteStatus.Pending
            },
            new TeamInvite
            {
                Id = Guid.NewGuid(),
                TeamId = team.Id,
                InvitedUserId = invited.Id,
                InvitedByUserId = inviter.Id,
                CreatedAt = DateTime.UtcNow,
                Status = InviteStatus.Accepted
            });
        await db.SaveChangesAsync();

        var result = await repo.GetMyPendingInvitesAsync(invited.Id, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(InviteStatus.Pending);
        result[0].Team.Should().NotBeNull();
        result[0].InvitedByUser.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Get_Team_Invites_As_Leader()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, inviter, team) = SeedUsersAndTeam(db);
        var requestingUserId = Guid.NewGuid();
        
        db.TeamInvites.Add(new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = inviter.Id,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        });
        await db.SaveChangesAsync();
        
        await db.TeamMembers.AddAsync(new TeamMember { TeamId = team.Id, UserId = requestingUserId });
        await db.SaveChangesAsync();
        
        memberRepo.MarkLeader(team.Id, requestingUserId);

        var result = await repo.GetTeamInvitesAsync(team.Id, requestingUserId, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_Get_Team_Invites_As_User_Who_Sent_Them()
    {
        await using var db = CreateDbContext();
        var memberRepo = new TestTeamMemberRepository(db);
        var repo = new TeamInviteRepository(db, memberRepo);

        var (invited, _, team) = SeedUsersAndTeam(db);
        var requestingUserId = Guid.NewGuid();


        db.Users.Add(new User
        {
            Id = requestingUserId, 
            UserName = "requester", 
            DisplayName = "Requester User",
            Email = "r@test", 
            PasswordHash = "z"
        });

        db.TeamInvites.Add(new TeamInvite
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            InvitedUserId = invited.Id,
            InvitedByUserId = requestingUserId,
            CreatedAt = DateTime.UtcNow,
            Status = InviteStatus.Pending
        });
        await db.SaveChangesAsync();


        await db.TeamMembers.AddAsync(new TeamMember { TeamId = team.Id, UserId = requestingUserId });
        await db.SaveChangesAsync();

        var result = await repo.GetTeamInvitesAsync(team.Id, requestingUserId, CancellationToken.None);

        result.Should().HaveCount(1);
        result.All(x => x.InvitedByUserId == requestingUserId).Should().BeTrue();
    }

}
