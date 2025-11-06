using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Data;

public sealed class JamSpaceDbContext : DbContext
{
    public JamSpaceDbContext(DbContextOptions<JamSpaceDbContext> options)
        : base(options)
    {
    }

    
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TeamInvite> TeamInvites => Set<TeamInvite>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<UserGenre> UserGenres => Set<UserGenre>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    
    public new async Task SaveChangesAsync(CancellationToken ct) => await base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new TeamInviteConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new UserGenreConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new UserSkillConfiguration());
        modelBuilder.ApplyConfiguration(new UserFollowConfiguration());
    }
    
}