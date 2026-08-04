using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(256);

        builder.Property(u => u.AuthProvider)
            .HasMaxLength(50);

        builder.Property(u => u.AuthProviderId)
            .HasMaxLength(128);

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => new { u.AuthProvider, u.AuthProviderId })
            .HasDatabaseName("IX_Users_AuthProvider");

        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.Property<uint>("xmin").IsRowVersion();

        builder.HasMany(u => u.ProjectMemberships)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}