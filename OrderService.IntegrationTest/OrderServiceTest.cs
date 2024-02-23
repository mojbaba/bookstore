using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.TestingTools;
using InventoryService.QueryBooks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderService.CreateOrder;
using OrderService.Entities;
using UserService;

namespace TokenService.IntegrationTest;

[Collection("default collection")]
public class OrderServiceTest
{
    private readonly OrderServiceHostFixture _orderServiceHostFixture;

    public OrderServiceTest(OrderServiceHostFixture orderServiceHostFixture)
    {
        _orderServiceHostFixture = orderServiceHostFixture;
    }

    private void MockInventoryClient(QueryBookResponse queryBookResponse)
    {
        _orderServiceHostFixture.InventoryClientMock
            .Setup(a => a.QueryBooksAsync(It.IsAny<QueryBookRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryBookResponse);
    }

    [Fact]
    public async Task CreateOrder_WithSuccessPaymentAndInventory_ShouldSuccess()
    {
        // Arrange
        var client = _orderServiceHostFixture.CreateClient();

        var configuration = _orderServiceHostFixture.Configuration;

        var jwtTokenService = new JwtTokenService(configuration);

        var userId = Guid.NewGuid().ToString();

        var userEmail = "user@example.com";

        var token = jwtTokenService.GenerateToken(userEmail, userId);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var bookIds = new List<string>
        {
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        };

        var request = new CreateOrderControllerRequest()
        {
            BookIds = bookIds
        };

        var kafkaOptions = _orderServiceHostFixture.Services.GetRequiredService<KafkaOptions>();

        MockInventoryClient(
            new QueryBookResponse()
            {
                Books = bookIds.Select(a => new Book()
                {
                    BookId = a,
                    Price = 100,
                    Author = "author",
                    Title = "title"
                }).ToList()
            });


        var inventoryFastConsumer = new KafkaFastConsumer<OrderCreatedEvent>(kafkaOptions.BootstrapServers,
            kafkaOptions.Topics.OrderCreatedTopic, "inventory-service-integration-test");

        var paymentFastConsumer = new KafkaFastConsumer<OrderCreatedEvent>(kafkaOptions.BootstrapServers,
            kafkaOptions.Topics.OrderCreatedTopic, "payment-service-integration-test");

        var ctx = new CancellationTokenSource();

        // Act
        var inventoryTask = inventoryFastConsumer.ConsumeAsync(ctx.Token);
        var paymentTask = paymentFastConsumer.ConsumeAsync(ctx.Token);

        var response = await client.PostAsJsonAsync("/api/order/create", request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createOrderResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        await Task.Delay(150);
        ctx.Cancel();

        var inventoryEvents = await inventoryTask;
        var paymentEvents = await paymentTask;


        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(createOrderResponse);
        Assert.Equal(OrderStatus.Processing, createOrderResponse.Status);
        Assert.NotEmpty(createOrderResponse.OrderId);
        Assert.Equal(bookIds, createOrderResponse.BookIds);

        Assert.Single(inventoryEvents);
        Assert.Single(paymentEvents);

        var inventoryEvent = inventoryEvents.Single();
        var paymentEvent = paymentEvents.Single();

        Assert.Equal(createOrderResponse.OrderId, inventoryEvent.OrderId);
        Assert.Equal(createOrderResponse.OrderId, paymentEvent.OrderId);
        Assert.Equal(bookIds, inventoryEvent.BookIds);
        Assert.Equal(bookIds, paymentEvent.BookIds);


        var eventLogProducer = _orderServiceHostFixture.Services.GetRequiredService<IEventLogProducer>();

        var balanceDeductedSucceededEvent = new BalanceDeductedSucceededEvent
        {
            BookIds = bookIds,
            OrderId = inventoryEvent.OrderId,
            TotalPrice = inventoryEvent.TotalPrice
        };

        var orderedBooksPackedEvent = new OrderedBooksPackedEvent
        {
            BookIds = bookIds,
            OrderId = inventoryEvent.OrderId,
            TotalPrice = inventoryEvent.TotalPrice
        };

        await eventLogProducer.ProduceAsync(kafkaOptions.Topics.BalanceDeductedTopic, balanceDeductedSucceededEvent,
            CancellationToken.None);
        await eventLogProducer.ProduceAsync(kafkaOptions.Topics.BooksPackedTopic, orderedBooksPackedEvent,
            CancellationToken.None);

        await Task.Delay(200);

        var orderRepository = _orderServiceHostFixture.Services.GetRequiredService<IOrderRepository>();
        var order = await orderRepository.GetAsync(createOrderResponse.OrderId, CancellationToken.None);

        Assert.Equal(OrderStatus.Succeeded, order.Status);
        Assert.True(order.IsInventoryProcessed);
        Assert.True(order.IsPaymentProcessed);
    }
}