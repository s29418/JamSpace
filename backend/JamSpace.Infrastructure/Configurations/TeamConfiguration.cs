using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Team");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(t => t.CreatedAt);
        
        builder.Property(t => t.TeamPictureUrl)
            .HasMaxLength(255);
        
        // builder.Property(t => t.CreatedById)
        // .IsRequired();

        builder.HasQueryFilter(t => !t.CreatedBy.IsDeleted);

        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
