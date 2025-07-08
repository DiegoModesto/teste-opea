using SharedKernel;

namespace Domain;

public sealed class OutboxEvent : Entity
{
    public Guid Id { get; set; }
    public AggregateType Aggregate { get; set; }
    public EventType Event { get; set; }
    public string Payload { get; set; }
    public bool Processed { get; set; }
}
