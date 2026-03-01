using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Message");
        
        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(m => m.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Conversation)
            .WithMany()
            .HasForeignKey(m => m.ConversationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.SenderUser)
            .WithMany()
            .HasForeignKey(m => m.SenderUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}