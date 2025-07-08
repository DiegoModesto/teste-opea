using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Config;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder
            .Property(b => b.Title)
            .IsRequired();

        builder
            .Property(b => b.Author)
            .IsRequired();

        builder
            .Property(b => b.TotalRemaining)
            .HasDefaultValue(0)
            .IsRequired();

        builder
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder
            .Property(b => b.IsDeleted)
            .HasDefaultValue(false);
    }
}
