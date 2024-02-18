using BookStore.Authentication.Jwt;
using BookStore.EventObserver;
using UserService.Logout;

namespace UserService;

public class UserLoggedOutObserver : IEventPublishObserver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly int _expiryMinutes;

    public UserLoggedOutObserver(IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _expiryMinutes = configuration.GetValue<int>("Jwt:ExpiryMinutes");
    }

    public async Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserLoggedOutEvent userLogoutEvent)
        {
            return;
        }

        using var tokenValidationService = _serviceProvider.GetRequiredService<ITokenValidationService>();
        
        await tokenValidationService.RevokeTokenAsync(userLogoutEvent.Token, TimeSpan.FromMinutes(_expiryMinutes),
            CancellationToken.None);
    }
}
