using BookStore.Authentication.Jwt;
using BookStore.EventObserver;
using UserService.Logout;

namespace UserService;

public class TokenValidationServiceDecorator : ITokenValidationService, IEventPublishObserver
{
    private readonly ITokenValidationServiceFactory _tokenValidationServiceFactory;
    private readonly int _expiryMinutes;

    public TokenValidationServiceDecorator(ITokenValidationServiceFactory tokenValidationServiceFactory,
        IEventPublishObservant eventPublishObservant,
        IConfiguration configuration)
    {
        _tokenValidationServiceFactory = tokenValidationServiceFactory;
        _expiryMinutes = configuration.GetValue<int>("Jwt:ExpiryMinutes");
        eventPublishObservant.Subscribe(this);
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken)
    {
        using var tokenValidationService = _tokenValidationServiceFactory.Create();
        return await tokenValidationService.ValidateTokenAsync(token, cancellationToken);
    }

    public async Task RevokeTokenAsync(string token, TimeSpan expiry, CancellationToken cancellationToken)
    {
        using var tokenValidationService = _tokenValidationServiceFactory.Create();
        await tokenValidationService.RevokeTokenAsync(token, expiry, cancellationToken);
    }

    public async Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserLoggedOutEvent userLogoutEvent)
        {
            return;
        }

        using var tokenValidationService = _tokenValidationServiceFactory.Create();
        
        await tokenValidationService.RevokeTokenAsync(userLogoutEvent.Token, TimeSpan.FromMinutes(_expiryMinutes),
            CancellationToken.None);
    }

    public void Dispose()
    {
    }
}

public static class TokenValidationServiceDecoratorRegisteration
{
    public static void AddTokenValidationServiceDecorator(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                 typeof(ITokenValidationService));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton<ITokenValidationService, TokenValidationServiceDecorator>();
    }
}

