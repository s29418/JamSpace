using FluentAssertions;
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

        var db = new JamSpaceDbContext(opts);
        db.Database.EnsureCreated();
        return db;
    }

    private static (Team team, User user) SeedMember(
        JamSpaceDbContext db,
        FunctionalRole role = FunctionalRole.Member,
        string musicalRole = "guitar")
    {
        var team = new Team { Id = Guid.NewGuid(), Name = "Team A" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "alice",
            DisplayName = "Alice",
            Email = "a@test",
            PasswordHash = "x"
        };

        db.Teams.Add(team);
        db.Users.Add(user);
        db.TeamMembers.Add(new TeamMember
        {
            TeamId = team.Id,
            UserId = user.Id,
            Role = role,
            MusicalRole = musicalRole
        });

        db.SaveChanges();
        return (team, user);
    }

    [Fact]
    public async Task HasRequiredRoleAsync_Returns_True_When_User_Is_In_Team_And_Meets_MinimumRole()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db, FunctionalRole.Admin);
        var repo = new TeamMemberRepository(db);

        (await repo.HasRequiredRoleAsync(team.Id, user.Id, FunctionalRole.Member, Ct)).Should().BeTrue();
        (await repo.HasRequiredRoleAsync(team.Id, user.Id, FunctionalRole.Admin, Ct)).Should().BeTrue();
        (await repo.HasRequiredRoleAsync(team.Id, user.Id, FunctionalRole.Leader, Ct)).Should().BeFalse();
    }

    [Fact]
    public async Task HasRequiredRoleAsync_Returns_False_When_User_Is_Not_In_Team()
    {
        await using var db = CreateDb();
        var (team, _) = SeedMember(db);
        var other = new User { Id = Guid.NewGuid(), UserName = "bob", DisplayName = "bob", Email = "b@test", PasswordHash = "y" };
        db.Users.Add(other);
        db.SaveChanges();

        var repo = new TeamMemberRepository(db);

        var ok = await repo.HasRequiredRoleAsync(team.Id, other.Id, FunctionalRole.Member, Ct);
        ok.Should().BeFalse();
    }

    [Fact]
    public async Task GetByTeamAndUserAsync_Returns_Member_With_Included_User()
    {
        await using var db = CreateDb();
        var (team, user) = SeedMember(db);
        var repo = new TeamMemberRepository(db);

        var member = await repo.GetByTeamAndUserAsync(team.Id, user.Id, Ct);

        member.Should().NotBeNull();
        member!.User.Should().NotBeNull();
        member.TeamId.Should().Be(team.Id);
        member.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByTeamAndUserAsync_Returns_Null_When_Missing()
    {
        await using var db = CreateDb();
        var repo = new TeamMemberRepository(db);

        var member = await repo.GetByTeamAndUserAsync(Guid.NewGuid(), Guid.NewGuid(), Ct);

        member.Should().BeNull();
    }

    [Fact]
    public async Task GetLeadersAsync_Returns_Only_Leaders()
    {
        await using var db = CreateDb();
        var (team, _) = SeedMember(db, FunctionalRole.Member);

        var l1 = new User { Id = Guid.NewGuid(), UserName = "l1", DisplayName = "l1", Email = "l1@test", PasswordHash = "x" };
        var l2 = new User { Id = Guid.NewGuid(), UserName = "l2", DisplayName = "l2", Email = "l2@test", PasswordHash = "x" };

        db.Users.AddRange(l1, l2);
        db.TeamMembers.AddRange(
            new TeamMember { TeamId = team.Id, UserId = l1.Id, Role = FunctionalRole.Leader },
            new TeamMember { TeamId = team.Id, UserId = l2.Id, Role = FunctionalRole.Leader }
        );
        db.SaveChanges();

        var repo = new TeamMemberRepository(db);

        var leaders = await repo.GetLeadersAsync(team.Id, Ct);

        leaders.Should().HaveCount(2);
        leaders.All(m => m.TeamId == team.Id && m.Role == FunctionalRole.Leader).Should().BeTrue();
    }

    [Fact]
    public async Task Add_And_Remove_Work_After_SaveChanges()
    {
        await using var db = CreateDb();
        var repo = new TeamMemberRepository(db);

        var team = new Team { Id = Guid.NewGuid(), Name = "T" };
        var user = new User { Id = Guid.NewGuid(), UserName = "u", DisplayName = "u", Email = "u@test", PasswordHash = "x" };
        db.Teams.Add(team);
        db.Users.Add(user);
        db.SaveChanges();

        var member = new TeamMember { TeamId = team.Id, UserId = user.Id, Role = FunctionalRole.Member };

        await repo.AddAsync(member, Ct);
        await db.SaveChangesAsync(Ct);

        db.TeamMembers.Any(m => m.TeamId == team.Id && m.UserId == user.Id).Should().BeTrue();

        repo.Remove(member);
        await db.SaveChangesAsync(Ct);

        db.TeamMembers.Any(m => m.TeamId == team.Id && m.UserId == user.Id).Should().BeFalse();
    }
}