using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(n => n.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(n => n.Message)
            .HasMaxLength(240)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.ReadAt)
            .IsRequired(false);

        builder.HasOne(n => n.RecipientUser)
            .WithMany()
            .HasForeignKey(n => n.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.ActorUser)
            .WithMany()
            .HasForeignKey(n => n.ActorUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.RecipientUserId);
        builder.HasIndex(n => n.ActorUserId);
        builder.HasIndex(n => n.ConversationId);
        builder.HasIndex(n => n.TeamId);
        builder.HasIndex(n => n.TeamInviteId);
        builder.HasIndex(n => n.TeamEventId);
        builder.HasIndex(n => n.PostId);
        builder.HasIndex(n => n.ProjectId);
        builder.HasIndex(n => n.ProjectNoteId);
        builder.HasIndex(n => n.CreatedAt);

        builder.HasIndex(n => new { n.RecipientUserId, n.IsRead, n.CreatedAt });
        builder.HasIndex(n => new { n.RecipientUserId, n.CreatedAt });
        builder.HasIndex(n => new { n.RecipientUserId, n.Type, n.ConversationId, n.IsRead });
    }
}
