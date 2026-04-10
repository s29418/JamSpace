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

        builder.Property(p => p.CreatedAt);

        builder.HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Media)
            .WithOne(pm => pm.Post)
            .HasForeignKey<PostMedia>(pm => pm.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}