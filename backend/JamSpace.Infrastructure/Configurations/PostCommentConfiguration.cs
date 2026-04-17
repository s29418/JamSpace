using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable("PostComment");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pc => pc.CreatedAt)
            .IsRequired();

        builder.HasOne(pc => pc.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(pc => pc.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.User)
            .WithMany(u => u.PostComments)
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pc => pc.PostId);

        builder.HasIndex(pc => pc.UserId);

        builder.HasIndex(pc => pc.CreatedAt);
    }
}