using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Project");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(p => p.PictureUrl)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}