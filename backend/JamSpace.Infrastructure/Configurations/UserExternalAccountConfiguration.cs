using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JamSpace.Infrastructure.Configurations;

public class UserExternalAccountConfiguration : IEntityTypeConfiguration<UserExternalAccount>
{
    public void Configure(EntityTypeBuilder<UserExternalAccount> builder)
    {
        builder.ToTable("UserExternalAccount");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Provider)
            .IsRequired();

        builder.Property(x => x.ExternalUserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ProfileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.AvatarUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.AccessToken)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(4000);

        builder.Property(x => x.Scopes)
            .HasMaxLength(1000);

        builder.Property(x => x.ConnectedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasQueryFilter(x => !x.User!.IsDeleted);

        builder.HasOne(x => x.User)
            .WithMany(u => u.ExternalAccounts)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new { x.UserId, x.Provider })
            .IsUnique()
            .HasFilter("[DisconnectedAt] IS NULL");

        builder.HasIndex(x => new { x.Provider, x.ExternalUserId })
            .IsUnique()
            .HasFilter("[DisconnectedAt] IS NULL");
    }
}
