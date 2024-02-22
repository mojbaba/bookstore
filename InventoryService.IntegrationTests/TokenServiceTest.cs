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
            Price = 10
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
        var removeBookResponse = await client.PostAsJsonAsync("/api/admin/remove-book",
            new { BookId = result.BookId });
        
        // Assert
        removeBookResponse.EnsureSuccessStatusCode();
        
        var bookRepository = _inventoryServiceHostFixture.Services.GetRequiredService<IBookRepository>();
        
        var bookEntity = await bookRepository.GetAsync(result.BookId, CancellationToken.None);
        
        Assert.Null(bookEntity);
    }
}