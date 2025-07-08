using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infra.Config;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);

        builder
            .Property(l => l.LoanDate)
            .IsRequired();

        builder
            .Property(l => l.ReturnDate)
            .IsRequired();

        builder
            .Property(l => l.Status)
            .IsRequired();

        builder
            .Property(l => l.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder
            .HasOne(l => l.Book)
            .WithOne(b => b.Loan)
            .HasForeignKey<Loan>(l => l.BookId);
    }
}
