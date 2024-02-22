using BookStore.EventObserver;

namespace UserService.Logout;

public record UserLoggedOutEvent : EventBase
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required DateTimeOffset Date { get; set; }
}