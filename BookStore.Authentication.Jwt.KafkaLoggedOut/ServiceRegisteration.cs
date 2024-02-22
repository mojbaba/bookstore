using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore.Authentication.Jwt.KafkaLoggedOut;

public static class ServiceRegisteration
{
    public static IServiceCollection AddKafkaUserLoggedOutHandler(this IServiceCollection services,
        Action<KafkaUserLoggedOutOptions> options)
    {
        services.Configure<KafkaUserLoggedOutOptions>(options);
        services.AddSingleton<KafkaUserLoggedOutHandler>();
        services.AddSingleton<IHostedService, KafkaUserLoggedoutConsumer>();
        return services;
    }
}