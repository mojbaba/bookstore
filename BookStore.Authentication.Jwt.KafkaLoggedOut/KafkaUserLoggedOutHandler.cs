using Microsoft.Extensions.Configuration;
using UserService.Logout;

namespace BookStore.Authentication.Jwt.KafkaLoggedOut;

public class KafkaUserLoggedOutHandler(ITokenValidationService tokenValidationService, IConfiguration configuration)
{
    public async Task HandleAsync(UserLoggedOutEvent userLoggedOutEvent, CancellationToken cancellationToken)
    {
        var expiry =  int.Parse(configuration["Jwt:ExpiryMinutes"]);
        await tokenValidationService.RevokeTokenAsync(userLoggedOutEvent.Token, TimeSpan.FromMinutes(expiry), cancellationToken);
    }
}