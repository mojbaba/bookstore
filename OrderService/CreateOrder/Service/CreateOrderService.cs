using BookStore.EventObserver;
using InventoryService.QueryBooks;
using OrderService.Entities;

namespace OrderService.CreateOrder;

public class CreateOrderService(
    IEventPublishObservant eventPublishObservant,
    IOrderRepository repository,
    IInventoryClient inventoryClient) : ICreateOrderService
{
    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = new OrderEntity
        {
            Status = OrderStatus.Processing
        };

        var queryBooksRequest = new QueryBookRequest
        {
            BookIds = request.BookIds
        };
        
        var queryBookResponse = await inventoryClient.QueryBooksAsync(queryBooksRequest, cancellationToken);

        var notFoundBookIds = request.BookIds.Except(queryBookResponse.Books.Select(book => book.BookId)).ToList();

        if (notFoundBookIds.Any())
        {
            throw new CreateOrderException($"Books with ids {string.Join(", ", notFoundBookIds)} not found");
        }

        order.Items = queryBookResponse.Books.Select(book => new OrderItemEntity
        {
            BookId = book.BookId,
            Price = book.Price
        }).ToList();

        var totalPrice = order.Items.Sum(item => item.Price);
        
        
        await repository.CreateAsync(order, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);
        
        var createOrderEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            BookIds = order.Items.Select(item => item.BookId),
            TotalPrice = totalPrice
        };
        
        await eventPublishObservant.PublishAsync(createOrderEvent);
        
        return new CreateOrderResponse
        {
            Status = order.Status,
            OrderId = order.Id,
            BookIds = order.Items.Select(item => item.BookId)
        };
    }
}