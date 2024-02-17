using UserService.EventPublisher;

namespace UserService.Login;

public record UserLoggedinEvent : EventBase
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required DateTimeOffset Date { get; set; }
}