namespace DefaultNamespace;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Bio)
            .HasColumnType("text");

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(255);

        builder.Property(u => u.Location)
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamp");

        builder.Property(u => u.SpotifyUrl)
            .HasMaxLength(255);

        builder.Property(u => u.SoundcloudUrl)
            .HasMaxLength(255);
        
    }
}