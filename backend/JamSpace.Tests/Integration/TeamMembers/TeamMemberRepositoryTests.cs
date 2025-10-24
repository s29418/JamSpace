using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using JamSpace.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Tests.Integration.TeamMembers;

public class TeamMemberRepositoryTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    private static JamSpaceDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<JamSpaceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new JamSpaceDbContext(opts);
    }

    private static (Team team, User user) SeedMember(
        JamSpaceDbContext db,
        FunctionalRole role = FunctionalRole.Member,
        string musicalRole = "guitar")
    {
        var team = new Team
        {
            Id = Guid.NewGuid(), 
            Name = "Team A"
        };
        var user = new User
        {
            Id = Guid.NewGuid(), 
            UserName = "alice", 
            DisplayName = "Alice",
            Email = "a@test", 
            PasswordHash = "x"
        };
        var member = new TeamMember
        {
            TeamId = team.Id,
            UserId = user.Id,
            Role = role,
            MusicalRole = musicalRole
        };

        db.Teams.Add(team);
        db.Users.Add(user);
        db.TeamMembers.Add(member);
        db.SaveChanges();

        return (team, user);
    }

    [Fact]
    public async Task IsUserInTeam_Returns_True_If_Member_Exists()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db);
        var repo = new TeamMemberRepository(db);

        var exists = await repo.IsUserInTeamAsync(team.Id, user.Id, Ct);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserInTeam_Returns_False_If_Not_In_Team()
    {
        await using var db = CreateDb();
        var (team, _) = SeedMember(db);
        var otherUser = new User
        {
            Id = Guid.NewGuid(), 
            UserName = "bob", 
            DisplayName = "bob",
            Email = "b@test", 
            PasswordHash = "y"
        };
        db.Users.Add(otherUser);
        db.SaveChanges();

        var repo = new TeamMemberRepository(db);

        var exists = await repo.IsUserInTeamAsync(team.Id, otherUser.Id, Ct);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task IsUserALeader_And_IsUserAnAdmin_Work_For_Roles()
    {
        await using var db = CreateDb();
        var (team, userLeader) = SeedMember(db, FunctionalRole.Leader);
        var adminUser = new User
        {
            Id = Guid.NewGuid(), 
            UserName = "admin", 
            DisplayName = "admin",
            Email = "admin@test", 
            PasswordHash = "z"
        };
        db.Users.Add(adminUser);
        db.TeamMembers.Add(new TeamMember
        {
            TeamId = team.Id, 
            UserId = adminUser.Id, 
            Role = FunctionalRole.Admin
        });
        db.SaveChanges();

        var repo = new TeamMemberRepository(db);

        (await repo.IsUserALeaderAsync(team.Id, userLeader.Id, Ct)).Should().BeTrue();
        (await repo.IsUserAnAdminAsync(team.Id, userLeader.Id, Ct)).Should().BeFalse();

        (await repo.IsUserAnAdminAsync(team.Id, adminUser.Id, Ct)).Should().BeTrue();
        (await repo.IsUserALeaderAsync(team.Id, adminUser.Id, Ct)).Should().BeFalse();
    }

    [Fact]
    public async Task GetTeamMember_Returns_Entity_With_Included_User()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db);
        var repo = new TeamMemberRepository(db);

        var member = await repo.GetTeamMemberAsync(team.Id, user.Id, CancellationToken.None);

        member.Should().NotBeNull();
        member.TeamId.Should().Be(team.Id);
        member.UserId.Should().Be(user.Id);
        member.User.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTeamMember_Throws_NotFound_If_Missing()
    {
        await using var db = CreateDb();
        var repo = new TeamMemberRepository(db);

        Func<Task> act = () => repo.GetTeamMemberAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Team member not found*");
    }

    [Fact]
    public async Task GetLeaders_Returns_Only_Leaders_For_Team()
    {
        await using var db = CreateDb();
        var (team, _) = SeedMember(db);
        var leader1 = new User
        {
            Id = Guid.NewGuid(), 
            UserName = "l1", 
            DisplayName = "l1",
            Email = "l1@test", 
            PasswordHash = "x"
        };
        var leader2 = new User
        {
            Id = Guid.NewGuid(), 
            UserName = "l2", 
            DisplayName = "l2",
            Email = "l2@test", 
            PasswordHash = "x"
        };
        db.Users.AddRange(leader1, leader2);
        db.TeamMembers.AddRange(
            new TeamMember
            {
                TeamId = team.Id, 
                UserId = leader1.Id, 
                Role = FunctionalRole.Leader
            },
            new TeamMember
            {
                TeamId = team.Id, 
                UserId = leader2.Id, 
                Role = FunctionalRole.Leader
            });
        db.SaveChanges();

        var repo = new TeamMemberRepository(db);

        var leaders = await repo.GetLeadersAsync(team.Id, CancellationToken.None);

        leaders.Should().HaveCount(2);
        leaders.All(m => m.Role == FunctionalRole.Leader && m.TeamId == team.Id).Should().BeTrue();
    }

    [Fact]
    public async Task ChangeFunctionalRole_Updates_Role_And_Saves()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db);
        var repo = new TeamMemberRepository(db);

        var updated = await repo.ChangeTeamMemberFunctionalRoleAsync(
            team.Id, user.Id, FunctionalRole.Admin, CancellationToken.None);

        updated.Role.Should().Be(FunctionalRole.Admin);
        var reloaded = db.TeamMembers.Single(m => m.TeamId == team.Id && m.UserId == user.Id);
        reloaded.Role.Should().Be(FunctionalRole.Admin);
    }

    [Fact]
    public async Task EditMusicalRole_Updates_Field_And_Saves()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db, musicalRole: "guitar");
        var repo = new TeamMemberRepository(db);

        var updated = await repo.EditTeamMemberMusicalRole(
            team.Id, user.Id, "drums", CancellationToken.None);

        updated.MusicalRole.Should().Be("drums");
        db.TeamMembers.Single(m => m.TeamId == team.Id && m.UserId == user.Id)
            .MusicalRole.Should().Be("drums");
    }

    [Fact]
    public async Task Delete_Removes_Member()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db);
        var repo = new TeamMemberRepository(db);

        await repo.DeleteTeamMemberAsync(team.Id, user.Id, CancellationToken.None);

        db.TeamMembers.Any(m => m.TeamId == team.Id && m.UserId == user.Id).Should().BeFalse();
    }
}
