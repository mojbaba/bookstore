namespace BookStore.Authentication.Jwt.Redis;

public class RedisTokenValidationServiceFactory (string connectionString): ITokenValidationServiceFactory
{
    public ITokenValidationService Create()
    {
        return new RedisTokenValidationService(connectionString);
    }
}