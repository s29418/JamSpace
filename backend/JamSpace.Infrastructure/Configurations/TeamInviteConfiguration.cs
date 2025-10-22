using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class TeamInviteConfiguration : IEntityTypeConfiguration<TeamInvite>
{
    public void Configure(EntityTypeBuilder<TeamInvite> builder)
    {
        builder.ToTable("TeamInvite");

        builder.HasKey(ti => ti.Id);

        builder.Property(ti => ti.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ti => ti.CreatedAt);
        
        builder.HasQueryFilter(ti => !ti.InvitedUser.IsDeleted);
        builder.HasQueryFilter(ti => !ti.InvitedByUser.IsDeleted);

        builder.HasOne(ti => ti.Team)
            .WithMany()
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ti => ti.InvitedUser)
            .WithMany()
            .HasForeignKey(ti => ti.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ti => ti.InvitedByUser)
            .WithMany()
            .HasForeignKey(ti => ti.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(ti => new { ti.TeamId, ti.InvitedUserId, ti.Status })
            .HasFilter("[Status] = 'Pending'")
            .IsUnique();
    }
}
