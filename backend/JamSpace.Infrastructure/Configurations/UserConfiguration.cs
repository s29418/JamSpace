using JamSpace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JamSpace.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired();

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Bio)
            .HasColumnType("text");

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(255);
        
        builder.Property(u => u.CreatedAt);

        builder.Property(u => u.SpotifyUrl)
            .HasMaxLength(255);

        builder.Property(u => u.SoundcloudUrl)
            .HasMaxLength(255);
        
        builder.OwnsOne(u => u.Location, loc =>
        {
            var upperStr = new ValueConverter<string?, string?>(
                v => v == null ? null : v.ToUpperInvariant(),
                v => v 
            );
            
            loc.Property(p => p.CountryCode)
                .HasMaxLength(2)
                .IsRequired()
                .HasConversion(upperStr);
            
            loc.Property(p => p.StateCode)
                .HasMaxLength(2)
                .HasConversion(upperStr);
            
            loc.Property(p => p.City)
                .HasMaxLength(128);
            
            loc.HasIndex(p => p.CountryCode)
                .HasDatabaseName("IX_User_Loc_Country");

            loc.HasIndex(p => new { p.CountryCode, p.City })
                .HasDatabaseName("IX_User_Loc_Country_City");

            loc.HasIndex(p => new { p.CountryCode, p.StateCode, p.City })
                .HasDatabaseName("IX_User_Loc_US_State_City");
            
            loc.HasIndex(p => p.City)
                .HasDatabaseName("IX_User_Loc_City");
        });
    }
}