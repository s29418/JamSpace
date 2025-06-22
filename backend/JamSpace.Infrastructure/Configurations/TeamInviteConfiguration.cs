namespace DefaultNamespace;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TeamInviteConfiguration : IEntityTypeConfiguration<TeamInvite>
{
    public void Configure(EntityTypeBuilder<TeamInvite> builder)
    {
        builder.ToTable("TeamInvite");

        builder.HasKey(ti => ti.Id);

        builder.Property(ti => ti.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ti => ti.CreatedAt)
            .HasColumnType("timestamp");

        builder.HasOne(ti => ti.Team)
            .WithMany()
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ti => ti.InvitedUser)
            .WithMany()
            .HasForeignKey(ti => ti.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ti => ti.InvitedByUser)
            .WithMany()
            .HasForeignKey(ti => ti.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
