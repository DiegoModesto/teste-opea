using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel;

namespace Infra.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Where(e => e.Entity is not OutboxEvent)
            .ToList();
    
        if (entries.Count == 0)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    
        var outboxEvents = new List<OutboxEvent>(entries.Count);
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    
        foreach (EntityEntry entry in entries)
        {
            EventType eventType = entry.State == EntityState.Added ? EventType.Created : EventType.Updated;
            AggregateType aggregateType = Enum.Parse<AggregateType>(entry.Entity.GetType().Name);
    
            entry.Entity.GetType().GetProperty(name: "CreatedAt")?.SetValue(entry.Entity, DateTimeOffset.UtcNow);
            
            var outboxEvent = new OutboxEvent
            {
                Id = Guid.NewGuid(),
                Aggregate = aggregateType,
                Event = eventType,
                Payload = JsonSerializer.Serialize(entry.Entity, jsonOptions),
                Processed = false
            };
    
            outboxEvents.Add(outboxEvent);
        }
    
        OutboxEvents.AddRange(outboxEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }
}
