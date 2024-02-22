using StackExchange.Redis;

namespace BookStore.Authentication.Jwt.Redis;

public class RedisTokenValidationService(IDatabase database) : ITokenValidationService
{
    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken)
    {
        var keyExists = await database.KeyExistsAsync(token);
        var valid = !keyExists;
        return valid;
    }

    public Task RevokeTokenAsync(string token, TimeSpan expiry, CancellationToken cancellationToken)
    {
        return database.StringSetAsync(token, "revoked", expiry);
    }

    public void Dispose()
    {
    }
}