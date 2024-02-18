using Microsoft.Extensions.DependencyInjection;

namespace BookStore.EventObserver;

public static class ServiceRegisteration
{
    public static void RegisterEventSourceObservant(this IServiceCollection services)
    {
        services.AddSingleton<IEventPublishObservant, EventSourcePublish>();
    }
}