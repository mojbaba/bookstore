using BookStore.EventObserver;
using Microsoft.AspNetCore.Identity;

namespace UserService.Register;

public class UserRegisterService(IUserRepository repository, IEventPublishObservant eventPublishObservant)
    : IUserRegisterService
{
    public async Task<UserRegisterationResponse> RegisterAsync(UserRegisterationRequest request,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetUserAsync(request.Email, cancellationToken);
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

        var createdUser = await repository.CreateUserAsync(newUser, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

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