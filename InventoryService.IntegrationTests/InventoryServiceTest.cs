using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.TestingTools;
using InventoryService.AdminAddBook.Models;
using InventoryService.Entities;
using InventoryService.IntegrationTest;
using InventoryService.QueryBooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.CreateOrder;
using UserService;

namespace TokenService.IntegrationTest;

[Collection("default collection")]
public class InventoryServiceTest
{
    private readonly InventoryServiceHostFixture _inventoryServiceHostFixture;

    public InventoryServiceTest(InventoryServiceHostFixture inventoryServiceHostFixture)
    {
        _inventoryServiceHostFixture = inventoryServiceHostFixture;
    }


    [Fact]
    public async Task CatchOrderCreatedEvent_ShouldSendPackedEvent()
    {
        // Arrange
        var client = _inventoryServiceHostFixture.CreateClient();
        var eventLogProducer = _inventoryServiceHostFixture.Services.GetRequiredService<IEventLogProducer>();
        var kafkaOptions = _inventoryServiceHostFixture.Services.GetRequiredService<KafkaOptions>();

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            TotalPrice = 75,
            BookIds = new[]
            {
                Guid.NewGuid().ToString()
            }
        };

        await eventLogProducer.ProduceAsync(kafkaOptions.Topics.OrderCreatedTopic, orderCreatedEvent,
            CancellationToken.None);

        await Task.Delay(5000);
        
        var fastConsumer = new KafkaFastConsumer<OrderedBooksPackedEvent>(kafkaOptions.BootstrapServers,
            kafkaOptions.Topics.BooksPackedTopic,
            "token-service-test-group");
        
        var packedEvents = await fastConsumer
            .ConsumeAsync(new CancellationTokenSource(5000).Token);
        
        
        // Assert
        Assert.NotNull(packedEvents);
        Assert.NotEmpty(packedEvents);
        Assert.Equal(orderCreatedEvent.OrderId, packedEvents.First().OrderId);
        Assert.Equal(orderCreatedEvent.BookIds, packedEvents.First().BookIds);
        Assert.Equal(orderCreatedEvent.TotalPrice, packedEvents.First().TotalPrice);
    }

    [Fact]
    public async Task CreateBook_ShouldBeSuccessful()
    {
        // Arrange
        var client = _inventoryServiceHostFixture.CreateClient();

        var book = new AdminAddBookRequest()
        {
            Title = "Test Book",
            Author = "Test Author",
            Price = 10,
            Amount = 15
        };

        // Act
        var config = _inventoryServiceHostFixture.Services.GetRequiredService<IConfiguration>();
        var tokenService = new JwtTokenService(config);

        var userId = Guid.NewGuid().ToString();
        var userEmail = "user@example.com";

        var token = tokenService.GenerateToken(userId, userEmail);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/admin/add-book", book);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AdminAddBookResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.NotEmpty(result.BookId);

        var bookQueryResponse = await client.GetAsync("/api/query-books/query");
        var content = await bookQueryResponse.Content.ReadAsStringAsync();
        var bookQueryResult = await bookQueryResponse.Content.ReadFromJsonAsync<QueryBookResponse>();

        Assert.Contains(book.Title, bookQueryResult.Books.Select(x => x.Title));

        // Act
        var removeBookResponse = await client.PostAsJsonAsync("/api/admin/remove-book",
            new { BookId = result.BookId });

        // Assert
        removeBookResponse.EnsureSuccessStatusCode();

        var bookRepository = _inventoryServiceHostFixture.Services.GetRequiredService<IBookRepository>();

        var bookEntity = await bookRepository.GetAsync(result.BookId, CancellationToken.None);

        Assert.Null(bookEntity);
    }
}