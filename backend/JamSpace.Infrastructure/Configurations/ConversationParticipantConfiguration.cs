using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipant");

        builder.HasKey(cp => new { cp.ConversationId, cp.UserId });
        
        builder.HasIndex(cp => cp.UserId);

        builder.Property(cp => cp.Role)
            .IsRequired(false)
            .HasMaxLength(25);

        builder.Property(cp => cp.LastReadMessageId)
            .IsRequired(false);

        builder.Property(cp => cp.LastReadAt)
            .IsRequired(false);

        builder.HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.Conversation)
            .WithMany()
            .HasForeignKey(cp => cp.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}