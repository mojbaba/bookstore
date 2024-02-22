using System.Net.Http.Headers;
using System.Net.Http.Json;
using TokenService.AddToken;
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
        
        var token = jwtTokenService.GenerateToken(userEmail,userId);
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await client.PostAsJsonAsync("/api/book-purchase-token/add", new {Amount = 100}, CancellationToken.None);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AddBookPurchaseTokenResponse>();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(userId, result.UserId);
        
        // Act
        var removeResponse = await client.PostAsJsonAsync("/api/book-purchase-token/remove", new {Amount = 50}, CancellationToken.None);
        var removeStringResponse = await removeResponse.Content.ReadAsStringAsync();
        var removeResult = await removeResponse.Content.ReadFromJsonAsync<RemoveBookPurchaseTokenResponse>();
        
        // Assert
        Assert.NotNull(removeResult);
        Assert.True(removeResponse.IsSuccessStatusCode);
        Assert.Equal(userId, removeResult.UserId);
        Assert.Equal(50, removeResult.UpdatedBalance);
        Assert.Equal(50, removeResult.RemovedAmount);
    }
}