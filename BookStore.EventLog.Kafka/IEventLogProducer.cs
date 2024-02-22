using BookStore.EventObserver;

namespace BookStore.EventLog.Kafka;

public interface IEventLogProducer
{
    Task ProduceAsync<TEvent>(string channelName, TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent;
}