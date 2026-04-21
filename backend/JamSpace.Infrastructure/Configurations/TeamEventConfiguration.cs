using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class TeamEventConfiguration : IEntityTypeConfiguration<TeamEvent>
{
    public void Configure(EntityTypeBuilder<TeamEvent> builder)
    {
        builder.ToTable("TeamEvent");

        builder.HasKey(te => te.Id);

        builder.Property(te => te.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(te => te.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(te => te.StartDateTime)
            .IsRequired();

        builder.Property(te => te.DurationMinutes)
            .IsRequired();

        builder.Property(te => te.CreatedAt)
            .IsRequired();

        builder.HasOne(te => te.Team)
            .WithMany(t => t.Events)
            .HasForeignKey(te => te.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.CreatedBy)
            .WithMany()
            .HasForeignKey(te => te.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(te => te.TeamId);

        builder.HasIndex(te => te.CreatedById);

        builder.HasIndex(te => te.StartDateTime);

        builder.HasIndex(te => new { te.TeamId, te.StartDateTime });
    }
}