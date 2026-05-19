using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ProjectNoteConfiguration : IEntityTypeConfiguration<ProjectNote>
{
    public void Configure(EntityTypeBuilder<ProjectNote> builder)
    {
        builder.ToTable("ProjectNote");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.StartTimeSeconds)
            .IsRequired(false);

        builder.Property(n => n.EndTimeSeconds)
            .IsRequired(false);

        builder.Property(n => n.Status)
            .IsRequired()
            .HasDefaultValue(ProjectNoteStatus.Active);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .IsRequired();

        builder.Property(n => n.CompletedAt)
            .IsRequired(false);

        builder.HasOne(n => n.Project)
            .WithMany(p => p.Notes)
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.AudioVersion)
            .WithMany(v => v.Notes)
            .HasForeignKey(n => n.AudioVersionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(n => n.CreatedBy)
            .WithMany()
            .HasForeignKey(n => n.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.CompletedBy)
            .WithMany()
            .HasForeignKey(n => n.CompletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.ProjectId);
        builder.HasIndex(n => n.AudioVersionId);
        builder.HasIndex(n => n.CreatedById);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => new { n.ProjectId, n.Status, n.CreatedAt });
    }
}
