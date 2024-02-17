namespace UserService;

public interface ITokenValidationService
{
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken);
}