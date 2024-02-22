using BookStore.EventObserver;

namespace BookStore.EventLog.Kafka;

public interface IEventLogProducer
{
    Task ProduceAsync<TEvent>(string channelName, IEvent @event, CancellationToken cancellationToken);
}