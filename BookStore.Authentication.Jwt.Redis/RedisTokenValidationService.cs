using StackExchange.Redis;

namespace BookStore.Authentication.Jwt.Redis;

public class RedisTokenValidationService(string connectionString) : ITokenValidationService
{
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(connectionString);

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var keyExists = await db.KeyExistsAsync(token);
        var valid = !keyExists;
        return valid;
    }

    public Task RevokeTokenAsync(string token, TimeSpan expiry, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        return db.StringSetAsync(token, "revoked", expiry);
    }

    public void Dispose()
    {
        _redis.Dispose();
    }
}