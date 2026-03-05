using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversation");
        
        builder.HasIndex(c => c.LastMessageAt);

        builder.HasIndex(x => x.DirectKey)
            .IsUnique()
            .HasFilter("[DirectKey] IS NOT NULL");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.TeamId)
            .IsRequired(false);

        builder.Property(x => x.DirectKey)
            .HasMaxLength(80)
            .IsRequired(false);
        
        builder.Property(x => x.LastMessageAt)
            .IsRequired(false);

        builder.Property(x => x.LastMessagePreview)
            .HasMaxLength(120)
            .IsRequired(false);

        builder.Property(x => x.LastMessageId)
            .IsRequired(false);

        builder.HasMany(c => c.Participants)
            .WithOne(cp => cp.Conversation)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.Team)
            .WithMany()
            .HasForeignKey(c => c.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}