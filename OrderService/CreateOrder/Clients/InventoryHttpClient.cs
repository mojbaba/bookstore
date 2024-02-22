using InventoryService.QueryBooks;

namespace OrderService.CreateOrder;

public class InventoryHttpClient(HttpClient httpClient) : IInventoryClient
{
    public async Task<QueryBookResponse> QueryBooksAsync(QueryBookRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/books/query", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QueryBookResponse>(cancellationToken: cancellationToken);
    }
}