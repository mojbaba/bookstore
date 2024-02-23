using System.Net.Http.Headers;
using System.Net.Http.Json;
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
    

}