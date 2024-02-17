using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.EventPublisher;

namespace UserService.Login;

public class UserLoginService(UserServiceDbContext dbContext , ITokenService tokenService, IUserLoginEventSource userLoginEventSource) : IUserLoginService
{
    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
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
        
        var token = tokenService.GenerateToken(user.Id);
        
        await userLoginEventSource.PublishAsync(new UserLoggedinEvent
        {
            Id = user.Id,
            Email = user.Email,
            Date = DateTimeOffset.Now
        });
        
        return new UserLoginResponse
        {
            Email = user.Email,
            Token = token
        };
        
    }
}