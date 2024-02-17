using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.EventPublisher;

namespace UserService.Register;

public class UserRegisterService(UserServiceDbContext dbContext, IUserRegisterEventSource eventPublishObservant) : IUserRegisterService
{
    public async Task<UserRegisterationResponse> RegisterAsync(UserRegisterationRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email,cancellationToken);
        if (user != null)
        {
            throw new UserRegisterException("Email is already registered");
        }
        
        var passwordHasher = new PasswordHasher<UserEntity>();
        var randomSalt = Guid.NewGuid().ToString();
        var passwordHash = passwordHasher.HashPassword(null, request.Password + randomSalt);
        
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Password = passwordHash,
            Salt = randomSalt
        };
        
        dbContext.Users.Add(newUser);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        await eventPublishObservant.PublishAsync(new Events.UserRegisteredEvent
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Date = DateTimeOffset.Now
        });
        
        return new UserRegisterationResponse
        {
            Email = newUser.Email
        };
    }
}