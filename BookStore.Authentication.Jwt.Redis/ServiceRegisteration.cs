using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Authentication.Jwt.Redis;

public static class ServiceRegisteration
{
    public static void AddRedisTokenValidationService(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<ITokenValidationService, RedisTokenValidationService>(c=> new RedisTokenValidationService(connectionString));
        services.AddTransient<ITokenValidationServiceFactory, RedisTokenValidationServiceFactory>(c=> new RedisTokenValidationServiceFactory(connectionString));
    }   
}