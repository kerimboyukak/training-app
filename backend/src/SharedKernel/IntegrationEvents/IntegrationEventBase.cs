namespace IntegrationEvents;

public abstract class IntegrationEventBase : IIntegrationEvent      // base class for all our integration events
{
    public Guid EventId { get; }
    public DateTime CreationDate { get; }

    protected IntegrationEventBase()
    {
        EventId = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}