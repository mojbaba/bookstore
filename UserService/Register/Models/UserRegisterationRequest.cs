using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserService.Register;

public class UserRegisterationRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    
    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }
}