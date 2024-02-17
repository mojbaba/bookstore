namespace UserService.Login;

public record UserLoggedinEvent
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required DateTimeOffset Date { get; set; }
}