using System.Net.Http.Headers;
using System.Net.Http.Json;
using InventoryService.AdminAddBook.Models;
using InventoryService.Entities;
using InventoryService.IntegrationTest;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task CreateBook_ShouldBeSuccessful()
    {
        // Arrange
        var client = _inventoryServiceHostFixture.CreateClient();

        var book = new AdminAddBookRequest()
        {
            Title = "Test Book",
            Author = "Test Author",
            Price = 10,
            Amount = 10
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/admin/add-book", book);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<AdminAddBookResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(result);
        Assert.NotEmpty(result.BookId);

        // Act
        var increaseAmountResponse = await client.PostAsJsonAsync("/api/admin/change-book-amount",
            new { BookId = result.BookId, Amount = 10 });
        
        var changeAmountResult = await increaseAmountResponse.Content.ReadFromJsonAsync<AdminChangeBookAmountResponse>();
         
        // Assert
        increaseAmountResponse.EnsureSuccessStatusCode();
        Assert.NotNull(changeAmountResult);
        Assert.Equal(20, changeAmountResult.UpdatedAmount);
        
        // Act
        var decreaseAmountResponse = await client.PostAsJsonAsync("/api/admin/change-book-amount",
            new { BookId = result.BookId, Amount = -5 });
        
        var decreaseAmountResult = await decreaseAmountResponse.Content.ReadFromJsonAsync<AdminChangeBookAmountResponse>();
        
        // Assert
        decreaseAmountResponse.EnsureSuccessStatusCode();
        Assert.NotNull(decreaseAmountResult);
        Assert.Equal(15, decreaseAmountResult.UpdatedAmount);
        
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