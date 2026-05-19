using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ProjectAudioVersionConfiguration : IEntityTypeConfiguration<ProjectAudioVersion>
{
    public void Configure(EntityTypeBuilder<ProjectAudioVersion> builder)
    {
        builder.ToTable("ProjectAudioVersion");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(v => v.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Length)
            .IsRequired();

        builder.Property(v => v.DurationSeconds)
            .IsRequired(false);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.HasOne(v => v.Project)
            .WithMany(p => p.AudioVersions)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.CreatedBy)
            .WithMany()
            .HasForeignKey(v => v.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.ProjectId);
        builder.HasIndex(v => v.CreatedById);
        builder.HasIndex(v => new { v.ProjectId, v.CreatedAt });
    }
}
