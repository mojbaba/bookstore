using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Authentication.Jwt.Redis;

public static class ServiceRegisteration
{
    public static void AddRedisTokenValidationService(this IServiceCollection services)
    {
        services.AddTransient<ITokenValidationService, RedisTokenValidationService>();
    }
}