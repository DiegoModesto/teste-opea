using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Config;

public sealed class OutboxConfiguration: IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Aggregate)
            .IsRequired();

        builder
            .Property(e => e.Event)
            .IsRequired();

        builder
            .Property(e => e.Payload)
            .IsRequired();

        builder
            .Property(e => e.Processed)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder
            .Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}
