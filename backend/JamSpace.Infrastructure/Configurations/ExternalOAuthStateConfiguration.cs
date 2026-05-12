using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class ExternalOAuthStateConfiguration : IEntityTypeConfiguration<ExternalOAuthState>
{
    public void Configure(EntityTypeBuilder<ExternalOAuthState> builder)
    {
        builder.ToTable("ExternalOAuthState");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired();

        builder.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.CodeVerifier)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.ReturnUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.HasQueryFilter(x => !x.User!.IsDeleted);

        builder.HasOne(x => x.User)
            .WithMany(u => u.ExternalOAuthStates)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.State)
            .IsUnique();

        builder.HasIndex(x => x.ExpiresAt);

        builder.HasIndex(x => new { x.Provider, x.State });
    }
}
