using UserService.EventPublisher;

namespace UserService.Logout;

public record UserLoggedoutEvent : EventBase
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required DateTimeOffset Date { get; set; }
}