using UserService.EventPublisher;
using UserService.Logout;

namespace UserService;

public class InMemoryTokenValidationService : ITokenValidationService, IEventPublishObserver
{
    record RevokedToken(string Token, DateTimeOffset Date);

    private readonly TimeSpan _expiry;

    private Queue<RevokedToken> _revokedTokensQueue = new();

    private HashSet<string> _revokedTokens = new();

    public InMemoryTokenValidationService(IConfiguration configuration, IEventPublishObservant eventPublishObservant)
    {
        var expiryMinutes = configuration.GetValue<int>("Jwt:ExpiryMinutes");
        _expiry = TimeSpan.FromMinutes(expiryMinutes);
        eventPublishObservant.Subscribe(this);
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken)
    {
        RemoveExpiredTokens();

        if (_revokedTokens.Contains(token))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void RemoveExpiredTokens()
    {
        while (_revokedTokensQueue.Count > 0)
        {
            var token = _revokedTokensQueue.Peek();
            if (token.Date < DateTimeOffset.Now.Subtract(_expiry))
            {
                _revokedTokensQueue.Dequeue();
                _revokedTokens.Remove(token.Token);
            }
            else
            {
                break;
            }
        }
    }

    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserLoggedOutEvent userLoggedOutEvent)
        {
            return Task.CompletedTask;
        }
        
        if(_revokedTokens.Contains(userLoggedOutEvent.Token))
        {
            return Task.CompletedTask;
        }

        _revokedTokensQueue.Enqueue(new RevokedToken(userLoggedOutEvent.Token, userLoggedOutEvent.Date));
        _revokedTokens.Add(userLoggedOutEvent.Token);

        return Task.CompletedTask;
    }
}