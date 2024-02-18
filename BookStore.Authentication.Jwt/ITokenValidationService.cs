namespace BookStore.Authentication.Jwt;

public interface ITokenValidationService : IDisposable
{
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken);
    
    Task RevokeTokenAsync(string token, TimeSpan expiry, CancellationToken cancellationToken);
}

public interface ITokenValidationServiceFactory
{
    ITokenValidationService Create();
}