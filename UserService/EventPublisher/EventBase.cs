namespace UserService.EventPublisher;

public record EventBase
{
    public Guid EventId { get; } = Guid.NewGuid();
}