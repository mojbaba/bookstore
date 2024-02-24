using System.Net.Http.Headers;
using InventoryService.QueryBooks;
using Microsoft.AspNetCore.Http.Extensions;

namespace OrderService.CreateOrder;

public class InventoryHttpClient(IServiceProvider provider) : IInventoryClient
{
    public async Task<QueryBookResponse> QueryBooksAsync(QueryBookRequest request, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", request.AuthenticationToken);

        var configuration = provider.GetRequiredService<IConfiguration>();
        httpClient.BaseAddress = new Uri(configuration["Urls:InventoryService"]);

        var queryBuilder = new QueryBuilder();

        queryBuilder.Add("bookIds", request.BookIds);

        var parameters = queryBuilder.ToQueryString().ToString();

        var response = await httpClient.GetAsync("api/query-books/query" + parameters, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QueryBookResponse>(cancellationToken: cancellationToken);
    }
}