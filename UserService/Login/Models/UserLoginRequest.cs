using System.ComponentModel.DataAnnotations;

namespace UserService.Login;

public record UserLoginRequest
{
    [EmailAddress] public string Email { get; set; }
    public string Password { get; set; }
}