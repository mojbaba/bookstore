using BookStore.EventObserver;
using Microsoft.AspNetCore.Identity;

namespace UserService.Login;

public class UserLoginService(IUserRepository repository , ITokenService tokenService, IEventPublishObservant eventPublishObservant) : IUserLoginService
{
    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserAsync(request.Email, cancellationToken);
        if (user == null)
        {
            throw new UserLoginException("Invalid email or password");
        }
        
        var passwordHasher = new PasswordHasher<UserEntity>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, user.Password, request.Password + user.Salt);
        
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UserLoginException("Invalid email or password");
        }
        
        var token = tokenService.GenerateToken(user.Email,user.Id);
        
        await eventPublishObservant.PublishAsync(new UserLoggedinEvent
        {
            Id = user.Id,
            Email = user.Email,
            Date = DateTimeOffset.Now
        });
        
        return new UserLoginResponse
        {
            Email = user.Email,
            Token = $"Bearer {token}"
        };
        
    }
}