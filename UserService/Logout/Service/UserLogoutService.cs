using UserService.EventPublisher;

namespace UserService.Logout;

public class UserLogoutService(IEventPublishObservant eventPublishObservant) : IUserLogoutService
{
    public async Task<bool> LogoutAsync(UserLogoutRequest request, CancellationToken cancellationToken)
    {
        await eventPublishObservant.PublishAsync(new UserLoggedoutEvent
        {
            Date = DateTimeOffset.Now,
            Email = request.Email,
            Id = request.Id,
            Token = request.Token
        });

        return true;
    }
}