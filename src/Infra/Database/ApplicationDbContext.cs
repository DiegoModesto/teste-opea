using System.Text.Json;
using Application.Abstractions.Data;
using Domain;
using Microsoft.EntityFrameworkCore;
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

        foreach (OutboxEvent? outboxEvent in from entry in entries 
                 let eventType = entry.State == EntityState.Added ? EventType.Created : EventType.Updated 
                 let aggregateType = Enum.Parse<AggregateType>(entry.Entity.GetType().Name)
                select new OutboxEvent
                 {
                     Id = Guid.NewGuid(),
                     Aggregate = aggregateType,
                     Event = eventType,
                     Payload = JsonSerializer.Serialize(entry.Entity),
                     Processed = false
                 })
        {
            OutboxEvents.Add(outboxEvent);
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
