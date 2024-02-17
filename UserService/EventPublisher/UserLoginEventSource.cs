using UserService.Login;

namespace UserService.EventPublisher;

public interface IUserLoginEventSource : IEventPublishObservant<UserLoggedinEvent>
{
    
}

public class UserLoginEventSource : EventSourcePublishBase<UserLoggedinEvent>, IUserLoginEventSource
{
    
}
