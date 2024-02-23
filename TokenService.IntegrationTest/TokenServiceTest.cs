using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.TestingTools;
using Microsoft.Extensions.DependencyInjection;
using OrderService.CreateOrder;
using TokenService.AddToken;
using TokenService.Entities;
using TokenService.RemoveToken;
using UserService;

namespace TokenService.IntegrationTest;

[Collection("default collection")]
public class TokenServiceTest
{
    private readonly TokenServiceHostFixture _tokenServiceHostFixture;

    public TokenServiceTest(TokenServiceHostFixture tokenServiceHostFixture)
    {
        _tokenServiceHostFixture = tokenServiceHostFixture;
    }

    [Fact]
    public async Task AddBookPurchaseToken_ShouldSuccess()
    {
        // Arrange
        var client = _tokenServiceHostFixture.CreateClient();

        var configuration = _tokenServiceHostFixture.Configuration;

        var jwtTokenService = new JwtTokenService(configuration);

        var userId = Guid.NewGuid().ToString();

        var userEmail = "user@example.com";

        var token = jwtTokenService.GenerateToken(userEmail, userId);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/api/book-purchase-token/add", new { Amount = 100 },
            CancellationToken.None);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AddBookPurchaseTokenResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(userId, result.UserId);

        // Act
        var removeResponse = await client.PostAsJsonAsync("/api/book-purchase-token/remove", new { Amount = 50 },
            CancellationToken.None);
        var removeStringResponse = await removeResponse.Content.ReadAsStringAsync();
        var removeResult = await removeResponse.Content.ReadFromJsonAsync<RemoveBookPurchaseTokenResponse>();

        // Assert
        Assert.NotNull(removeResult);
        Assert.True(removeResponse.IsSuccessStatusCode);
        Assert.Equal(userId, removeResult.UserId);
        Assert.Equal(50, removeResult.UpdatedBalance);
        Assert.Equal(50, removeResult.RemovedAmount);
    }

    [Fact]
    public async Task
        ConsumeOrderCreatedEvent_WithEnoughBalance_ShouldBeOk_BalanceShouldBeUpdated_EventShouldPublished()
    {
        // Arrange
        var client = _tokenServiceHostFixture.CreateClient();

        var configuration = _tokenServiceHostFixture.Configuration;

        var jwtTokenService = new JwtTokenService(configuration);

        var userId = Guid.NewGuid().ToString();

        var userEmail = "user@example.com";

        var token = jwtTokenService.GenerateToken(userEmail, userId);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/api/book-purchase-token/add", new { Amount = 500 },
            CancellationToken.None);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AddBookPurchaseTokenResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(userId, result.UserId);

        // Act
        var eventLogProducer = _tokenServiceHostFixture.Services.GetRequiredService<IEventLogProducer>();
        var kafkaOptions = _tokenServiceHostFixture.Services.GetRequiredService<KafkaOptions>();

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid().ToString(),
            UserId = userId,
            TotalPrice = 75,
            BookIds = new[]
            {
                Guid.NewGuid().ToString()
            }
        };

        await eventLogProducer.ProduceAsync(kafkaOptions.Topics.OrderCreatedTopic, orderCreatedEvent,
            CancellationToken.None);

        await Task.Delay(5000);

        var fastConsumer = new KafkaFastConsumer<BalanceDeductedSucceededEvent>(kafkaOptions.BootstrapServers,
            kafkaOptions.Topics.BalanceDeductedTopic,
            "token-service-test-group");

        var balanceDeductedSucceededEvents = await fastConsumer
            .ConsumeAsync(new CancellationTokenSource(5000).Token);

        // Assert
        Assert.NotNull(balanceDeductedSucceededEvents);
        Assert.NotEmpty(balanceDeductedSucceededEvents);
        Assert.Equal(orderCreatedEvent.OrderId, balanceDeductedSucceededEvents.First().OrderId);
        Assert.Equal(orderCreatedEvent.BookIds, balanceDeductedSucceededEvents.First().BookIds);
        Assert.Equal(orderCreatedEvent.TotalPrice, balanceDeductedSucceededEvents.First().TotalPrice);
        
        var repository = _tokenServiceHostFixture.Services.GetRequiredService<IBookPurchaseTokenRepository>();
        var userToken = await repository.GetAsync(userId, new CancellationToken());
        
        Assert.NotNull(userToken);
        Assert.Equal(425, userToken.Amount);
        
        var historyRepository = _tokenServiceHostFixture.Services.GetRequiredService<IBookPurchaseTokenHistoryRepository>();
        var histories = await historyRepository.GetItemsByOrderIdAsync(orderCreatedEvent.OrderId, new CancellationToken());
        
        Assert.NotNull(histories);
        Assert.NotEmpty(histories);
        Assert.Equal(orderCreatedEvent.OrderId, histories.First().OrderId);
    }
    
    [Fact]
    public async Task
        ConsumeOrderCreatedEvent_WithInsufficientBalance_ShouldNotBeOk_BalanceShouldNotChange_FailEventShouldPublished()
    {
        // Arrange
        var client = _tokenServiceHostFixture.CreateClient();

        var configuration = _tokenServiceHostFixture.Configuration;

        var jwtTokenService = new JwtTokenService(configuration);

        var userId = Guid.NewGuid().ToString();

        var userEmail = "user@example.com";

        var token = jwtTokenService.GenerateToken(userEmail, userId);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/api/book-purchase-token/add", new { Amount = 10 },
            CancellationToken.None);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AddBookPurchaseTokenResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(userId, result.UserId);

        // Act
        var eventLogProducer = _tokenServiceHostFixture.Services.GetRequiredService<IEventLogProducer>();
        var kafkaOptions = _tokenServiceHostFixture.Services.GetRequiredService<KafkaOptions>();

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid().ToString(),
            UserId = userId,
            TotalPrice = 75,
            BookIds = new[]
            {
                Guid.NewGuid().ToString()
            }
        };

        await eventLogProducer.ProduceAsync(kafkaOptions.Topics.OrderCreatedTopic, orderCreatedEvent,
            CancellationToken.None);

        await Task.Delay(5000);

        var fastConsumer = new KafkaFastConsumer<BalanceDeductionFailedEvent>(kafkaOptions.BootstrapServers,
            kafkaOptions.Topics.BalanceDeductionFailedTopic,
            "token-service-test-group");

        var balanceDeductionFailedEvents = await fastConsumer
            .ConsumeAsync(new CancellationTokenSource(5000).Token);

        // Assert
        Assert.NotNull(balanceDeductionFailedEvents);
        Assert.NotEmpty(balanceDeductionFailedEvents);
        Assert.Equal(orderCreatedEvent.OrderId, balanceDeductionFailedEvents.First().OrderId);
        Assert.Equal(orderCreatedEvent.BookIds, balanceDeductionFailedEvents.First().BookIds);
        Assert.Equal(orderCreatedEvent.TotalPrice, balanceDeductionFailedEvents.First().TotalPrice);
        Assert.NotNull(balanceDeductionFailedEvents.First().Reason);
        
        var repository = _tokenServiceHostFixture.Services.GetRequiredService<IBookPurchaseTokenRepository>();
        var userToken = await repository.GetAsync(userId, new CancellationToken());
        
        Assert.NotNull(userToken);
        Assert.Equal(10, userToken.Amount);
        
        var historyRepository = _tokenServiceHostFixture.Services.GetRequiredService<IBookPurchaseTokenHistoryRepository>();
        var histories = await historyRepository.GetItemsByOrderIdAsync(orderCreatedEvent.OrderId, new CancellationToken());
        
        Assert.NotNull(histories);
        Assert.Empty(histories);
    }
    
}