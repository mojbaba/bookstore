namespace UserService.Logout;

public record UserLogoutRequest
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public required string Email { get; set; }
}