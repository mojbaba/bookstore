using UserService.Register.Events;

namespace UserService.EventPublisher;

public interface IUserRegisterEventSource : IEventPublishObservant<UserRegisteredEvent>
{
    
}

public class UserRegisterEventSource : EventSourcePublishBase<UserRegisteredEvent>, IUserRegisterEventSource
{
    
}