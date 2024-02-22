using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService;

public class UserEntity
{
    public string Id { get; set; } = new Guid().ToString();
    [Required] [MaxLength(200)] public string Email { get; set; }
    [Required] [MaxLength(300)] public string Password { get; set; }
    [Required] [MaxLength(300)] public string Salt { get; set; }
}