using System.Net.Http.Headers;
using InventoryService.QueryBooks;
using Microsoft.AspNetCore.Http.Extensions;

namespace OrderService.CreateOrder;

public class InventoryHttpClient(HttpClient httpClient) : IInventoryClient
{
    public async Task<QueryBookResponse> QueryBooksAsync(QueryBookRequest request, CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", request.AuthenticationToken);

        var queryBuilder = new QueryBuilder();

        queryBuilder.Add("bookIds", request.BookIds);

        var parameters = queryBuilder.ToQueryString().ToString();

        var response = await httpClient.GetAsync("api/books/query" + parameters, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QueryBookResponse>(cancellationToken: cancellationToken);
    }
}