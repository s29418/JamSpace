using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("UserFollows", t =>
        {
            t.HasCheckConstraint("CK_UserFollows_NoSelfFollow", "[FollowerId] <> [FollowedId]");
        });

        builder.HasKey(uf => new { uf.FollowerId, FollowedId = uf.FolloweeId });

        builder.Property(uf => uf.FollowedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uf => uf.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FolloweeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(uf => uf.FolloweeId)
            .HasDatabaseName("IX_UserFollows_FollowedId");
        
        builder.HasIndex(uf => uf.FollowerId)
            .HasDatabaseName("IX_UserFollows_FollowerId");
    }
}