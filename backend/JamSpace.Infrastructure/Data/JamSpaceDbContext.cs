using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Data;

public class JamSpaceDbContext : DbContext
{
    public JamSpaceDbContext(DbContextOptions<JamSpaceDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TeamInvite?> TeamInvites => Set<TeamInvite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new TeamInviteConfiguration());
    }
    
}