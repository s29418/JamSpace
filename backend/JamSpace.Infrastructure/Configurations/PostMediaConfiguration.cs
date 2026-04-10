using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ToTable("PostMedia");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Url)
            .IsRequired();

        builder.Property(pm => pm.MediaType)
            .IsRequired();

        builder.Property(pm => pm.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(pm => pm.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pm => pm.Length)
            .IsRequired();

        builder.HasIndex(pm => pm.PostId)
            .IsUnique();
    }
}