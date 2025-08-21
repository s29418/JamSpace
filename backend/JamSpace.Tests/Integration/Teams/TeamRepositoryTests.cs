using FluentAssertions;
using JamSpace.Application.Common.Exceptions;
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

        private static (User creator, Team team) SeedUserAndTeamShell(JamSpaceDbContext db)
        {
            var creator = new User
            {
                Id = Guid.NewGuid(),
                UserName = "creator",
                Email = "creator@test.com",
                PasswordHash = "zzz"
            };
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "The Originals",
                CreatedById = creator.Id,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(creator);
            db.SaveChanges();

            return (creator, team);
        }

        private static User SeedUser(JamSpaceDbContext db, string username = "user")
        {
            var u = new User
            {
                Id = Guid.NewGuid(),
                UserName = username,
                Email = $"{username}@test.com",
                PasswordHash = "zzz"
            };
            db.Users.Add(u);
            db.SaveChanges();
            return u;
        }

        [Fact]
        public async Task CreateTeamAsync_Should_Create_Team_And_Add_Creator_As_Leader()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);

            var (creator, team) = SeedUserAndTeamShell(db);

            var id = await repo.CreateTeamAsync(team, creator.Id, Ct);

            id.Should().Be(team.Id);
            var created = await db.Teams
                .Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == id);
            
            created.Should().NotBeNull();
            created.Members.Should().HaveCount(1);
            created.Members.Single().Should().BeEquivalentTo(new
            {
                TeamId = team.Id,
                UserId = creator.Id,
                Role = FunctionalRole.Leader
            });
        }

        [Fact]
        public async Task GetTeamByIdAsync_Should_Return_Team_With_Navigation_Props()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);

            await repo.CreateTeamAsync(team, creator.Id, Ct);

            var fetched = await repo.GetTeamByIdAsync(team.Id, Ct);

            fetched.Should().NotBeNull();
            fetched.CreatedBy.Should().NotBeNull();
            fetched.Members.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTeamByIdAsync_Should_Throw_NotFound_When_Missing()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);

            Func<Task> act = async () => await repo.GetTeamByIdAsync(Guid.NewGuid(), Ct);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Team not found*");
        }

        [Fact]
        public async Task GetTeamsByUserIdAsync_Should_Return_Teams_Where_User_Is_Member()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);

            var user = SeedUser(db, "member");
            var (creator, t1) = SeedUserAndTeamShell(db);
            var (_, t2) = SeedUserAndTeamShell(db);
            var (_, t3) = SeedUserAndTeamShell(db);
            
            await repo.CreateTeamAsync(t1, creator.Id, Ct);
            await repo.CreateTeamAsync(t2, creator.Id, Ct);
            await repo.CreateTeamAsync(t3, creator.Id, Ct);
            
            db.TeamMembers.Add(new TeamMember
            {
                TeamId = t1.Id, 
                UserId = user.Id, 
                Role = FunctionalRole.Member
            });
            db.TeamMembers.Add(new TeamMember
            {
                TeamId = t3.Id, 
                UserId = user.Id, 
                Role = FunctionalRole.Admin
            });
            await db.SaveChangesAsync();

            var result = await repo.GetTeamsByUserIdAsync(user.Id, CancellationToken.None);

            result.Select(x => x.Id).Should().BeEquivalentTo(new[] { t1.Id, t3.Id });
            result.All(x => x.Members.Any(m => m.UserId == user.Id)).Should().BeTrue();
        }

        [Fact]
        public async Task ChangeTeamNameAsync_Should_Update_Name()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);

            await repo.CreateTeamAsync(team, creator.Id, Ct);

            var newName = "New Grooves";
            var updated = await repo.ChangeTeamNameAsync(team.Id, newName, CancellationToken.None);

            updated.Name.Should().Be(newName);
            (await db.Teams.FindAsync(team.Id))!.Name.Should().Be(newName);
        }

        [Fact]
        public async Task UpdateTeamPictureAsync_Should_Succeed_For_Leader()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);

            await repo.CreateTeamAsync(team, creator.Id, Ct);

            var url = "https://cdn.example.com/pic.png";
            await repo.UpdateTeamPictureAsync(team.Id, creator.Id, url, CancellationToken.None);

            (await db.Teams.FindAsync(team.Id))!.TeamPictureUrl.Should().Be(url);
        }

        [Fact]
        public async Task UpdateTeamPictureAsync_Should_Succeed_For_Admin()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);
            await repo.CreateTeamAsync(team, creator.Id, Ct);

            var admin = SeedUser(db, "adminUser");
            db.TeamMembers.Add(new TeamMember
            {
                TeamId = team.Id, 
                UserId = admin.Id, 
                Role = FunctionalRole.Admin
            });
            await db.SaveChangesAsync();

            var url = "https://cdn.example.com/admin.png";
            await repo.UpdateTeamPictureAsync(team.Id, admin.Id, url, CancellationToken.None);

            (await db.Teams.FindAsync(team.Id))!.TeamPictureUrl.Should().Be(url);
        }

        [Fact]
        public async Task UpdateTeamPictureAsync_Should_Throw_Forbidden_For_Non_Admin_Or_Leader()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);
            await repo.CreateTeamAsync(team, creator.Id, Ct);

            var plainMember = SeedUser(db, "plainMember");
            db.TeamMembers.Add(new TeamMember
            {
                TeamId = team.Id, 
                UserId = plainMember.Id, 
                Role = FunctionalRole.Member
            });
            await db.SaveChangesAsync();

            Func<Task> act = async () =>
                await repo.UpdateTeamPictureAsync(team.Id, plainMember.Id, "x", CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenAccessException>()
                .WithMessage("*Only team leader or admin*");
        }

        [Fact]
        public async Task DeleteTeamAsync_Should_Remove_Team()
        {
            await using var db = CreateDbContext();
            var repo = new TeamRepository(db);
            var (creator, team) = SeedUserAndTeamShell(db);
            await repo.CreateTeamAsync(team, creator.Id, Ct);

            await repo.DeleteTeamAsync(team.Id, CancellationToken.None);

            (await db.Teams.FindAsync(team.Id)).Should().BeNull();
            (await db.TeamMembers.Where(m => m.TeamId == team.Id).ToListAsync()).Should().BeEmpty();
        }
    }

