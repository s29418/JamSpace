using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class PortfolioTrackConfiguration : IEntityTypeConfiguration<PortfolioTrack>
{
    public void Configure(EntityTypeBuilder<PortfolioTrack> builder)
    {
        builder.ToTable("PortfolioTrack");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Source)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ArtistName)
            .HasMaxLength(255);

        builder.Property(x => x.AlbumTitle)
            .HasMaxLength(255);

        builder.Property(x => x.ArtworkUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ExternalTrackId)
            .HasMaxLength(255);

        builder.Property(x => x.ExternalUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.EmbedUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.FileUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted && !x.User!.IsDeleted);

        builder.HasOne(x => x.User)
            .WithMany(u => u.PortfolioTracks)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ExternalAccount)
            .WithMany()
            .HasForeignKey(x => x.ExternalAccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.DisplayOrder });
        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
        builder.HasIndex(x => new { x.Source, x.ExternalTrackId });
        builder.HasIndex(x => x.ExternalAccountId);
    }
}
