using FluentAssertions;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using JamSpace.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Tests.Integration.Teams;

public class TeamRepositoryTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    private static JamSpaceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<JamSpaceDbContext>()
            .UseInMemoryDatabase(databaseName: $"jamspace_teams_{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        var db = new JamSpaceDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    private static User SeedUser(JamSpaceDbContext db, string username = "user")
    {
        var u = new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            DisplayName = username,
            Email = $"{username}@test.com",
            PasswordHash = "zzz"
        };
        db.Users.Add(u);
        db.SaveChanges();
        return u;
    }

    private static Team SeedTeamGraph(JamSpaceDbContext db, User creator, string name = "The Originals")
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedById = creator.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Teams.Add(team);
        
        db.TeamMembers.Add(new TeamMember
        {
            TeamId = team.Id,
            UserId = creator.Id,
            Role = FunctionalRole.Leader
        });

        db.SaveChanges();
        return team;
    }

    [Fact]
    public async Task Add_Should_Add_Team_And_Persist_After_SaveChanges()
    {
        await using var db = CreateDbContext();
        var repo = new TeamRepository(db);

        var creator = SeedUser(db, "creator");

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "New Team",
            CreatedById = creator.Id,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(team, Ct);
        await db.SaveChangesAsync(Ct);

        (await db.Teams.FindAsync(team.Id))!.Name.Should().Be("New Team");
    }

    [Fact]
    public async Task Remove_Should_Remove_Team_After_SaveChanges()
    {
        await using var db = CreateDbContext();
        var repo = new TeamRepository(db);

        var creator = SeedUser(db, "creator");
        var team = SeedTeamGraph(db, creator);

        repo.Remove(team);
        await db.SaveChangesAsync(Ct);

        (await db.Teams.FindAsync(team.Id)).Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Team_With_Navigation_Props_When_Exist()
    {
        await using var db = CreateDbContext();
        var repo = new TeamRepository(db);

        var creator = SeedUser(db, "creator");
        var team = SeedTeamGraph(db, creator);

        var fetched = await repo.GetByIdAsync(team.Id, Ct);

        fetched.Should().NotBeNull();
        fetched!.CreatedBy.Should().NotBeNull();  
        fetched.Members.Should().NotBeNull();       
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Missing()
    {
        await using var db = CreateDbContext();
        var repo = new TeamRepository(db);

        var fetched = await repo.GetByIdAsync(Guid.NewGuid(), Ct);

        fetched.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_Should_Return_Teams_Where_User_Is_Member()
    {
        await using var db = CreateDbContext();
        var repo = new TeamRepository(db);

        var creator = SeedUser(db, "creator");
        var member = SeedUser(db, "member");

        var t1 = SeedTeamGraph(db, creator, "T1");
        var t2 = SeedTeamGraph(db, creator, "T2");
        var t3 = SeedTeamGraph(db, creator, "T3");
        
        db.TeamMembers.Add(new TeamMember { TeamId = t1.Id, UserId = member.Id, Role = FunctionalRole.Member });
        db.TeamMembers.Add(new TeamMember { TeamId = t3.Id, UserId = member.Id, Role = FunctionalRole.Admin });
        await db.SaveChangesAsync(Ct);

        var result = await repo.GetByUserIdAsync(member.Id, Ct);

        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { t1.Id, t3.Id });
    }
}