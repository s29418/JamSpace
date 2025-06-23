namespace DefaultNamespace;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Team");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.CreatedAt);
        
        builder.Property(t => t.TeamPictureUrl)
            .HasMaxLength(255);
        
        // builder.Property(t => t.CreatedById)
        //     .IsRequired();


        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
