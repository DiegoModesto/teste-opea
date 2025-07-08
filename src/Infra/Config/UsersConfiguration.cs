using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Config;

public sealed class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(80);
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(80);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(120);
        
        builder
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
