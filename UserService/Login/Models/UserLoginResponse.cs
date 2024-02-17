namespace UserService.Login;

public record UserLoginResponse
{
    public string Email { get; set; }
    public string Token { get; set; }
}