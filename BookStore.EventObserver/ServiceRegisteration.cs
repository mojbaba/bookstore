using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore.EventObserver;

public static class ServiceRegisteration
{
    public static void RegisterEventSourceObservant(this IServiceCollection services)
    {
        services.AddSingleton<IEventPublishObservant, EventSourcePublish>();
    }
    
    public static void SubscribeObservers(this IHost app)
    {
        var eventSource = app.Services.GetRequiredService<IEventPublishObservant>();
        var observer = app.Services.GetServices<IEventPublishObserver>();
        
        foreach (var o in observer)
        {
            eventSource.Subscribe(o);
        }
    }
}