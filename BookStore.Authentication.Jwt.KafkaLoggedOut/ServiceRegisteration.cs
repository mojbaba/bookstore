using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore.Authentication.Jwt.KafkaLoggedOut;

public static class ServiceRegisteration
{
    public static IServiceCollection AddKafkaUserLoggedOutHandler(this IServiceCollection services,
        Func<IServiceProvider, KafkaUserLoggedOutOptions> getOptions)
    {
        services.AddSingleton<KafkaUserLoggedOutHandler>();
        services.AddHostedService<KafkaUserLoggedoutConsumer>(p =>
        {
            var options = getOptions(p);
            var userLoggedOutHandler = p.GetRequiredService<KafkaUserLoggedOutHandler>();
            return new KafkaUserLoggedoutConsumer(options, userLoggedOutHandler);
        });
        return services;
    }
}