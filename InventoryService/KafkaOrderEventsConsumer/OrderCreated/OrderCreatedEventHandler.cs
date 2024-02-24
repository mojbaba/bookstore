using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.RedisLock;
using InventoryService.Entities;
using OrderService.CreateOrder;

namespace InventoryService.KafkaOrderEventsConsumer;

public class OrderCreatedEventHandler(
    IEventLogProducer eventLogProducer,
    KafkaOptions kafkaOptions)
{
    public async Task HandleAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken)
    {
        // it must be implemented to reserve the stock for the order
        // if the stock is not enough, it must send an event to the order service to cancel the order
        // if the stock is enough, it must send an event to the order service to tell the payment is processed

        //currently this is a dummy implementation, and sends a success event: OrderedBooksPackEvent
        Console.WriteLine("OrderCreatedEventHandler: Handling OrderCreatedEvent");

        try
        {
            var successEvent = new OrderedBooksPackedEvent()
            {
                OrderId = orderCreatedEvent.OrderId,
                TotalPrice = orderCreatedEvent.TotalPrice,
                BookIds = orderCreatedEvent.BookIds
            };

            await SendSuccessEvent(successEvent, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Task SendSuccessEvent(OrderedBooksPackedEvent successEvent,
        CancellationToken cancellationToken)
    {
        //send an event to the order service to cancel the order
        Console.WriteLine("OrderCreatedEventHandler: Sending OrderedBooksPackedEvent");
        return eventLogProducer.ProduceAsync(kafkaOptions.Topics.BooksPackedTopic,
            successEvent,
            cancellationToken);
    }

    private Task SendFailureEvent(OrderedBooksPackingFailedEvent failedEvent,
        CancellationToken cancellationToken)
    {
        //send an event to the order service to tell the payment is processed
        return eventLogProducer.ProduceAsync(kafkaOptions.Topics.BooksPackingFailedTopic,
            failedEvent,
            cancellationToken);
    }
}