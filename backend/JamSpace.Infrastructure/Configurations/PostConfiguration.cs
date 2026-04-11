using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.OriginalPostId)
            .IsRequired(false);

        builder.HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Media)
            .WithOne(pm => pm.Post)
            .HasForeignKey<PostMedia>(pm => pm.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.OriginalPost)
            .WithMany(p => p.Reposts)
            .HasForeignKey(p => p.OriginalPostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Likes)
            .WithOne(pl => pl.Post)
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Comments)
            .WithOne(pc => pc.Post)
            .HasForeignKey(pc => pc.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.AuthorId);

        builder.HasIndex(p => p.CreatedAt);

        builder.HasIndex(p => p.OriginalPostId);

        builder.HasIndex(p => new { p.AuthorId, p.OriginalPostId })
            .IsUnique()
            .HasFilter("[OriginalPostId] IS NOT NULL");
    }
}